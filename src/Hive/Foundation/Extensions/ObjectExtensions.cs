using System;

namespace Hive.Foundation.Extensions
{
	public static class ObjectExtensions
	{
		public static TOut SelectOrDefault<TIn, TOut>(this TIn value, Func<TIn, TOut> resultIfNotNull, Func<TOut> resultIfNull = null)
			where TIn : class
		{
			if (value != null)
				return resultIfNotNull(value);

			return resultIfNull != null ? resultIfNull() : default(TOut);
		}

		public static TOut SelectOrDefault<TIn, TOut>(this TIn? value, Func<TIn, TOut> resultIfNotNull, Func<TOut> resultIfNull = null)
			where TIn : struct
		{
			if (value.HasValue)
				return resultIfNotNull(value.Value);

			return resultIfNull != null ? resultIfNull() : default(TOut);
		}
	}
}