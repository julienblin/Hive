using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Hive.Exceptions;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.Queries;
using Hive.Telemetry;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace Hive.Azure.DocumentDb
{
	public class DocumentDbQuery : Query
	{
		private readonly DocumentDbEntityRepository _repository;

		public DocumentDbQuery(DocumentDbEntityRepository repository, IEntityDefinition entityDefinition) : base(entityDefinition)
		{
			_repository = repository.NotNull(nameof(repository));
		}

		public override async Task<IEnumerable> ToEnumerable(CancellationToken ct)
		{
			var options = new FeedOptions
			{
				MaxItemCount = 50
			};

			var querySpec = BuildQuerySpec(this);
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
						{ "Query", querySpec.QueryText },
						{ "Parameters", string.Join(", ", querySpec.Parameters.Select(x => $"{x.Name} = {x.Value}")) }
					}
				);

			return docs.Select(x => _repository.ConvertToEntity(EntityDefinition, x));
		}

		public override async Task<object> UniqueResult(CancellationToken ct)
		{
			if(IsIdQuery)
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

		private SqlQuerySpec BuildQuerySpec(DocumentDbQuery query)
		{
			var parameters = new SqlParameterCollection();

			return new SqlQuerySpec($"SELECT * FROM ROOT WHERE {BuildWhere(query, parameters)}", parameters);
		}

		private string BuildWhere(DocumentDbQuery query, SqlParameterCollection parameters)
		{
			var whereConditions = new List<string>
			{
				"ROOT." + DocumentDbConstants.EntityDefinitionProperty + " = @" + DocumentDbConstants.EntityDefinitionProperty
			};

			parameters.Add(new SqlParameter("@" + DocumentDbConstants.EntityDefinitionProperty, query.EntityDefinition.FullName));

			foreach (var criterion in Criterions)
			{
				switch (criterion.Operator)
				{
					case Operators.Eq:
						whereConditions.Add($"ROOT.{criterion.PropertyName} = @{criterion.PropertyName}");
						parameters.Add(new SqlParameter($"@{criterion.PropertyName}", criterion.Value));
						break;
					case Operators.In:
						whereConditions.Add($"ROOT.{criterion.PropertyName} IN ({string.Join(", ", GetInValues(criterion.Value))})");
						break;
					default:
						throw new QueryException(this, $"Unsupported operator {criterion.Operator}.");
				}
			}

			return string.Join(" AND ", whereConditions);
		}

		private IEnumerable<object> GetInValues(object values)
		{
			if (values is IEnumerable)
			{
				foreach (var value in (IEnumerable) values)
				{
					yield return value is string ? $"\"{value}\"" : value;
				}
			}
			else
			{
				yield return values is string ? $"\"{values}\"" : values;
			}
		}
	}
}