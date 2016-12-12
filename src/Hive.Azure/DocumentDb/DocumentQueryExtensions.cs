using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hive.Queries;
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
				var batch = await docQuery.ExecuteNextAsync<T>(ct);

				batches.Add(batch);
			} while (docQuery.HasMoreResults);

			return batches.SelectMany(x => x);
		}

		public static async Task<IContinuationEnumerable<T>> ListContinuationAsync<T>(this IDocumentQuery<T> docQuery, CancellationToken ct)
		{
			var docs = await docQuery.ExecuteNextAsync<T>(ct);
			return new ContinuationEnumerable<T>(docs, docs.ResponseContinuation);
		}
	}
}