using System;

namespace Hive.Foundation.Exceptions
{
	/// <summary>
	/// Base class for Hive exceptions.
	/// </summary>
	public abstract class HiveException : Exception
	{
		protected HiveException() { }

		protected HiveException(string message) : base(message) { }

		protected HiveException(string message, Exception inner) : base(message, inner) { }
	}
}
