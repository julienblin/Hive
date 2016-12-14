using System;
using System.Globalization;

namespace Hive.Foundation.Extensions
{
	public static class StringExtensions
	{
		public static bool IsNullOrEmpty(this string value)
		{
			return string.IsNullOrEmpty(value);
		}

		public static bool IsNullOrWhiteSpace(this string value)
		{
			return string.IsNullOrWhiteSpace(value);
		}

		public static bool SafeOrdinalEquals(this string value, string other)
		{
			return string.Equals(value, other, StringComparison.Ordinal);
		}

		public static string SafeInvariantFormat(this string value, string format)
		{
			return value.IsNullOrEmpty() ? null : string.Format(CultureInfo.InvariantCulture, format, value);
		}

		public static int? IntSafeInvariantParse(this string value)
		{
			int result;
			return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result)
				? result
				: (int?) null;
		}

		public static T ToEnum<T>(this string value, bool ignoreCase = true)
		{
			if (value.IsNullOrEmpty())
				return default(T);

			return (T) Enum.Parse(typeof(T), value, ignoreCase);
		}

		public static string SplitFirst(this string value, char separator, out string remaining)
		{
			value.NotNullOrEmpty(nameof(value));
			var result = value.Split(new[] { separator }, 2, StringSplitOptions.RemoveEmptyEntries);
			remaining = (result.Length == 1) || (result[1].IsNullOrWhiteSpace()) ? null : result[1];
			return result[0];
		}
	}
}