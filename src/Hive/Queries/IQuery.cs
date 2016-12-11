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

		IQuery GetOrCreateSubQuery(string propertyName);

		IQuery AddOrder(Order order);

		bool IsIdQuery { get; }

		Task<IEnumerable> ToEnumerable(CancellationToken ct);

		Task<object> UniqueResult(CancellationToken ct);

		IEntityDefinition EntityDefinition { get; }
	}
}