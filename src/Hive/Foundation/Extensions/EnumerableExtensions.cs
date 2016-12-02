using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hive.Foundation.Extensions
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<T> Safe<T>(this IEnumerable<T> value)
		{
			return value ?? Enumerable.Empty<T>();
		}

		public static async Task<IEnumerable<TOut>> SafeForEachParallel<TIn, TOut>(this IEnumerable<TIn> value,
			Func<TIn, CancellationToken, Task<TOut>> func, CancellationToken ct)
		{
			if (value == null) return Enumerable.Empty<TOut>();

			var entityTasks = value.Select(x => func(x, ct)).ToList();
			await Task.WhenAll(entityTasks);
			return entityTasks.Select(x => x.Result);
		}

		public static TValue SafeGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			if (dictionary == null) return default(TValue);

			TValue value;
			return dictionary.TryGetValue(key, out value) ? value : default(TValue);
		}

		public static TValue SafeGet<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
		{
			if (dictionary == null) return default(TValue);

			TValue value;
			return dictionary.TryGetValue(key, out value) ? value : default(TValue);
		}
	}
}