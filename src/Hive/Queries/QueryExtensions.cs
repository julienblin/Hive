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

		public static async Task<T> UniqueResult<T>(this IQuery query, CancellationToken ct)
		{
			return (T)await query.UniqueResult(ct);
		}
	}
}