using System;
using System.Diagnostics;

namespace Hive.Foundation.Extensions
{
	public static class ArgsExtensions
	{
		[DebuggerStepThrough]
		public static T NotNull<T>(this T value, string name) where T : class
		{
			if (value == null)
				throw new ArgumentNullException(name);
			return value;
		}

		[DebuggerStepThrough]
		public static T Is<T>(this object value, string name)
		{
			if (value == null) return default(T);
			if(!(value is T))
				throw new ArgumentException($"Argument {name} ({value}) should be of type {typeof(T)}.", name);
			return (T)value;
		}

		[DebuggerStepThrough]
		public static string NotNullOrEmpty(this string value, string name)
		{
			if (value.IsNullOrEmpty())
				throw new ArgumentNullException(name);
			return value;
		}
	}
}