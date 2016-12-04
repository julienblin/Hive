using System;

namespace Hive.Exceptions
{
	public class HiveFatalException : HiveException
	{
		public HiveFatalException(string message) : base(message) { }

		public HiveFatalException(string message, Exception inner) : base(message, inner) { }
	}
}
