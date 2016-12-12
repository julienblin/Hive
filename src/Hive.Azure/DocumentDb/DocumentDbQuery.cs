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
				MaxItemCount = MaxResults
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

			var results = docs.Select(x => _repository.ConvertToEntity(EntityDefinition, x)).ToList();
			await ProcessIncludes(results, ct);
			return results;
		}

		public override async Task<IContinuationEnumerable> ToContinuationEnumerable(CancellationToken ct)
		{
			var options = new FeedOptions
			{
				MaxItemCount = MaxResults,
				RequestContinuation = ContinuationToken
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

					return docQuery.ListContinuationAsync(ct);
				},
				DependencyKind.HTTP,
				_repository.DependencyName,
				new Dictionary<string, string>
				{
					{"Query", querySpec.QueryText},
					{"Parameters", string.Join(", ", querySpec.Parameters.Select(x => $"{x.Name} = {x.Value}"))}
				}
			);

			var results = docs.Select(x => _repository.ConvertToEntity(EntityDefinition, x)).ToList();
			await ProcessIncludes(results, ct);
			return new ContinuationEnumerable(results, docs.ContinuationToken);
		}

		private async Task ProcessIncludes(List<IEntity> results, CancellationToken ct)
		{
			if (!Includes.Any()) return;

			//TODO: optimize / parallel processing
			foreach (var propertyDefinition in Includes)
			{
				var targetEntityDefinition =
					(IEntityDefinition) propertyDefinition.PropertyType.GetTargetValueType(propertyDefinition);

				var foreignIds = results
					.Select(x => x[propertyDefinition.Name])
					.Where(x => x != null)
					.Cast<IEntity>()
					.Select(x => _repository.GetDocumentId(targetEntityDefinition, x.Id))
					.Distinct();

				var foreignQuery = new DocumentDbQuery(_repository, targetEntityDefinition);
				foreignQuery.Add(Criterion.In(MetaConstants.IdProperty, foreignIds.ToArray()));
				var foreignEntities = (await foreignQuery.ToEnumerable<IEntity>(ct)).ToDictionary(x => x.Id);
				
				results.ForEach(x =>
				{
					var foreignId = ((IEntity) x[propertyDefinition.Name])?.Id;
					if (foreignId != null && foreignEntities.ContainsKey(foreignId))
						x[propertyDefinition.Name] = foreignEntities[foreignId];
				});
			}
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

			return new SqlQuerySpec($"SELECT VALUE ROOT FROM ROOT {await BuildWhere(query, parameters, ct)} {BuildOrder(query)}", parameters);
		}

		private static async Task<string> BuildWhere(DocumentDbQuery query, SqlParameterCollection parameters, CancellationToken ct)
		{
			var joins = new List<string>();

			var whereConditions = new List<string>
			{
				"ROOT." + DocumentDbConstants.EntityDefinitionProperty + " = @" + DocumentDbConstants.EntityDefinitionProperty
			};

			parameters.Add(new SqlParameter("@" + DocumentDbConstants.EntityDefinitionProperty, query.EntityDefinition.FullName));

			foreach (var criterion in query.Criterions)
			{
				string remaining;
				var leading = criterion.PropertyName.SplitFirst('.', out remaining);

				var propertyDefinition = query.EntityDefinition.Properties.SafeGet(leading);
				if(propertyDefinition == null)
					throw new QueryException(query, $"Unable to find a property named {leading} on {query.EntityDefinition}");

				var criteria = $"ROOT.{criterion.PropertyName}";
				if (propertyDefinition.PropertyType is ArrayValueType)
				{
					var prefix = GetPrefixName(propertyDefinition.Name);
					joins.Add($" JOIN {prefix} IN ROOT.{propertyDefinition.Name}");
					criteria = $"{prefix}.{remaining}";
				}

				switch (criterion.Operator)
				{
					case Operators.Eq:
						var parameterName = GetParameterName(criterion.PropertyName);
						whereConditions.Add($"{criteria} = @{parameterName}");
						parameters.Add(new SqlParameter($"@{parameterName}", criterion.Value));
						break;
					case Operators.In:
						whereConditions.Add($"{criteria} IN ({string.Join(", ", GetInValues(criterion.Value))})");
						break;
					default:
						throw new QueryException(query, $"Unsupported operator {criterion.Operator}.");
				}
			}

			foreach (var subQuery in query.SubQueries)
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

			return $"{string.Join(" ", joins)} WHERE {string.Join(" AND ", whereConditions)}";
		}

		private static string BuildOrder(DocumentDbQuery query)
		{
			if (!query.Orders.Any()) return null;

			if(query.Orders.Count > 1)
				throw new QueryException(query, "Document db does not support multiple order by.");

			var order = query.Orders[0];
			return $"ORDER BY ROOT.{order.PropertyName} {(order.Ascending ? "ASC" : "DESC")}";
		}

		private static string GetPrefixName(string propertyDefinitionName)
		{
			return $"prefix_{propertyDefinitionName}";
		}

		private static string GetParameterName(string propertyDefinitionName)
		{
			return propertyDefinitionName.Replace(".", "_");
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