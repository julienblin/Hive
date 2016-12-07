using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Linq;

namespace Hive.Azure.DocumentDb
{
	internal static class DocumentQueryExtensions
	{
		public static async Task<IEnumerable<T>> ListAsync<T>(this IDocumentQuery<T> docQuery, CancellationToken ct)
		{
			var batches = new List<IEnumerable<T>>();

			do
			{
				var batch = await docQuery.ExecuteNextAsync<T>();

				batches.Add(batch);
			}
			while (docQuery.HasMoreResults);

			return batches.SelectMany(x => x);
		}
	}
}