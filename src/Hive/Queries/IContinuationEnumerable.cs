using System.Collections;
using System.Collections.Generic;

namespace Hive.Queries
{
	public interface IContinuationEnumerable : IEnumerable
	{
		string ContinuationToken { get; }
	}

	public interface IContinuationEnumerable<out T> : IEnumerable<T>, IContinuationEnumerable
	{
	}
}