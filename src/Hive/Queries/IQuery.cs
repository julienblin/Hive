using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Hive.Meta;

namespace Hive.Queries
{
	public interface IQuery
	{
		/// <summary>
		/// Adds a simple property criterion.
		/// </summary>
		IQuery Add(ICriterion criterion);

		/// <summary>
		/// Create a sub-query for relations.
		/// The query returned is the subquery, not the original query.
		/// </summary>
		IQuery GetOrCreateSubQuery(string propertyName);

		IQuery AddOrder(Order order);

		IQuery SetMaxResults(int? maxResults);

		IQuery SetContinuationToken(string continuationToken);

		bool IsIdQuery { get; }

		int? MaxResults { get; }

		Task<IEnumerable> ToEnumerable(CancellationToken ct);

		Task<IContinuationEnumerable> ToContinuationEnumerable(CancellationToken ct);

		Task<object> UniqueResult(CancellationToken ct);

		IEntityDefinition EntityDefinition { get; }
	}
}