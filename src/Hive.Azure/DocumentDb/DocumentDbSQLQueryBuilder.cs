using System.Collections.Generic;
using Hive.Foundation.Extensions;
using Hive.Queries;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Hive.Azure.DocumentDb
{
	public class DocumentDbSqlQueryBuilder<T>
	{
		private readonly ListQuery _query;

		public DocumentDbSqlQueryBuilder(ListQuery query)
		{
			_query = query.NotNull(nameof(query));
		}

		public SqlQuerySpec GetSqlQuerySpec()
		{
			var parameters = new SqlParameterCollection();
			return new SqlQuerySpec($"SELECT {Select(parameters)} FROM ROOT WHERE {Where(parameters)}", parameters);
		}

		private string Select(SqlParameterCollection parameters)
		{
			if (_query.Limit.HasValue)
			{
				parameters.Add(new SqlParameter("@_limit", _query.Limit.Value));
				return "TOP @_limit *";
			}
			return "*";
		}

		private string Where(SqlParameterCollection parameters)
		{
			var whereConditions = new List<string>
			{
				"ROOT."+ DocumentDbConstants.EntityDefinitionProperty + " = @_entityType"
			};

			parameters.Add(new SqlParameter("@_entityType", _query.EntityDefinition.FullName));

			return string.Join(" AND ", whereConditions);
		}
	}
}