using System.Collections;
using System.Collections.Generic;
using Hive.Foundation.Extensions;

namespace Hive.Queries
{
	public class ContinuationEnumerable : IContinuationEnumerable
	{
		private readonly IEnumerable _innerCollection;

		public ContinuationEnumerable(IEnumerable innerCollection, string continuationToken)
		{
			_innerCollection = innerCollection.NotNull(nameof(innerCollection));
			ContinuationToken = continuationToken;
		}

		public IEnumerator GetEnumerator()
		{
			return _innerCollection.GetEnumerator();
		}

		public string ContinuationToken { get; }
	}

	public class ContinuationEnumerable<T> : IContinuationEnumerable<T>
	{
		private readonly IEnumerable<T> _innerCollection;

		public ContinuationEnumerable(IEnumerable<T> innerCollection, string continuationToken)
		{
			_innerCollection = innerCollection.NotNull(nameof(innerCollection));
			ContinuationToken = continuationToken;
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return _innerCollection.GetEnumerator();
		}

		public IEnumerator GetEnumerator()
		{
			return _innerCollection.GetEnumerator();
		}

		public string ContinuationToken { get; }
	}
}