using System;

namespace Hive.Foundation.Extensions
{
	public static class ArgsExtensions
	{
		public static T NotNull<T>(this T value, string name) where T : class
		{
			if (value == null)
				throw new ArgumentNullException(name);
			return value;
		}

		public static string NotNullOrEmpty(this string value, string name)
		{
			if (value.IsNullOrEmpty())
				throw new ArgumentNullException(name);
			return value;
		}
	}
}
