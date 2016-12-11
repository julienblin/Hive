using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace Hive.Queries
{
	public interface IQuery
	{
		IQuery Add(ICriterion criterion);

		IQuery AddOrder(Order order);

		bool IsIdQuery { get; }

		Task<IEnumerable> ToEnumerable(CancellationToken ct);

		Task<object> UniqueResult(CancellationToken ct);
	}
}