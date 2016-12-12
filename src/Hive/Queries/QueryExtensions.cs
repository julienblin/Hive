using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hive.Queries
{
	public static class QueryExtensions
	{
		public static async Task<IEnumerable<T>> ToEnumerable<T>(this IQuery query, CancellationToken ct)
		{
			return (await query.ToEnumerable(ct)).Cast<T>();
		}

		public static async Task<IContinuationEnumerable<T>> ToContinuationEnumerable<T>(this IQuery query, CancellationToken ct)
		{
			var queryResults = await query.ToContinuationEnumerable(ct);
			return new ContinuationEnumerable<T>(queryResults.Cast<T>(), queryResults.ContinuationToken);
		}

		public static async Task<T> UniqueResult<T>(this IQuery query, CancellationToken ct)
		{
			return (T)await query.UniqueResult(ct);
		}
	}
}