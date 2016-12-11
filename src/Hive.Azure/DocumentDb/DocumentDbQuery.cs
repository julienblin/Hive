using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Exceptions;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.Queries;
using Hive.Telemetry;
using Hive.ValueTypes;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Hive.Azure.DocumentDb
{
	public class DocumentDbQuery : Query
	{
		private readonly DocumentDbEntityRepository _repository;
		private static readonly HashSet<Type> UnquotedTypesIn = new HashSet<Type>
		{
			typeof(int),
			typeof(float),
			typeof(double),
			typeof(decimal),
			typeof(bool)
		};

		public DocumentDbQuery(DocumentDbEntityRepository repository, IEntityDefinition entityDefinition)
			: base(entityDefinition)
		{
			_repository = repository.NotNull(nameof(repository));
		}

		protected override IQuery InternalCreateSubQuery(IEntityDefinition entityDefinition)
		{
			return new DocumentDbQuery(_repository, entityDefinition);
		}

		public override async Task<IEnumerable> ToEnumerable(CancellationToken ct)
		{
			var options = new FeedOptions
			{
				MaxItemCount = 50
			};

			var querySpec = await BuildQuerySpec(this, ct);
			var docs = await _repository.Telemetry.TrackAsyncDependency(
				ct,
				_ =>
				{
					var docQuery = _repository
						.Client
						.CreateDocumentQuery<Document>(_repository.CollectionUri, querySpec, options)
						.AsDocumentQuery();

					return docQuery.ListAsync(ct);
				},
				DependencyKind.HTTP,
				_repository.DependencyName,
				new Dictionary<string, string>
				{
					{"Query", querySpec.QueryText},
					{"Parameters", string.Join(", ", querySpec.Parameters.Select(x => $"{x.Name} = {x.Value}"))}
				}
			);

			return docs.Select(x => _repository.ConvertToEntity(EntityDefinition, x));
		}

		public override async Task<object> UniqueResult(CancellationToken ct)
		{
			if (IsIdQuery)
			{
				try
				{
					var doc = await _repository.Telemetry.TrackAsyncDependency(
						ct,
						_ => _repository.Client.ReadDocumentAsync(_repository.GetDocumentUri(EntityDefinition, Criterions[0].Value)),
						DependencyKind.HTTP,
						_repository.DependencyName
					);
					return _repository.ConvertToEntity(EntityDefinition, doc);
				}
				catch (DocumentClientException ex)
				{
					if (ex.StatusCode == HttpStatusCode.NotFound)
						return null;
					throw;
				}
			}

			return (await ToEnumerable(ct)).Cast<object>().First();
		}

		private async Task<SqlQuerySpec> BuildQuerySpec(DocumentDbQuery query, CancellationToken ct)
		{
			var parameters = new SqlParameterCollection();

			return new SqlQuerySpec($"SELECT * FROM ROOT WHERE {await BuildWhere(query, parameters, ct)}", parameters);
		}

		private async Task<string> BuildWhere(DocumentDbQuery query, SqlParameterCollection parameters, CancellationToken ct)
		{
			var whereConditions = new List<string>
			{
				"ROOT." + DocumentDbConstants.EntityDefinitionProperty + " = @" + DocumentDbConstants.EntityDefinitionProperty
			};

			parameters.Add(new SqlParameter("@" + DocumentDbConstants.EntityDefinitionProperty, query.EntityDefinition.FullName));

			foreach (var criterion in Criterions)
			{
				string remaining;
				var leading = criterion.PropertyName.SplitFirst('.', out remaining);

				var propertyDefinition = query.EntityDefinition.Properties.SafeGet(leading);
				if(propertyDefinition == null)
					throw new QueryException(this, $"Unable to find a property named {leading} on {query.EntityDefinition}");

				switch (criterion.Operator)
				{
					case Operators.Eq:
						var parameterName = GetParameterName(criterion.PropertyName);
						whereConditions.Add($"ROOT.{criterion.PropertyName} = @{parameterName}");
						parameters.Add(new SqlParameter($"@{parameterName}", criterion.Value));
						break;
					case Operators.In:
						whereConditions.Add($"ROOT.{criterion.PropertyName} IN ({string.Join(", ", GetInValues(criterion.Value))})");
						break;
					default:
						throw new QueryException(this, $"Unsupported operator {criterion.Operator}.");
				}
			}

			foreach (var subQuery in SubQueries)
			{
				if (subQuery.Value.IsIdQuery)
				{
					whereConditions.Add($"ROOT.{subQuery.Key}.{MetaConstants.IdProperty} = @{subQuery.Key}");
					parameters.Add(new SqlParameter($"@{subQuery.Key}", ((DocumentDbQuery) subQuery.Value).Criterions[0].Value));
				}
				else
				{
					// We actually need to run the subquery at that point
					var subEntities = await ((DocumentDbQuery) subQuery.Value).ToEnumerable<IEntity>(ct);
					var subEntitiesIds = subEntities.Safe().Select(x => x.Id);
					whereConditions.Add(
						$"ROOT.{subQuery.Key}.{MetaConstants.IdProperty} IN ({string.Join(", ", GetInValues(subEntitiesIds))})");
				}
			}

			return string.Join(" AND ", whereConditions);
		}

		private string GetParameterName(string criterionPropertyName)
		{
			return criterionPropertyName.Replace(".", "_");
		}

		private static IEnumerable<object> GetInValues(object values)
		{
			if (values is IEnumerable)
			{
				foreach (var value in (IEnumerable) values)
				{
					yield return GetInValue(value);
				}
			}
			else
			{
				yield return GetInValue(values);
			}
		}

		private static object GetInValue(object value)
		{
			return UnquotedTypesIn.Contains(value.GetType()) ? value : $"\"{value}\"";
		}
	}
}