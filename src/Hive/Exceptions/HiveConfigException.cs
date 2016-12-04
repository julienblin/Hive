using System;

namespace Hive.Exceptions
{
	public class HiveConfigException : HiveFatalException
	{
		public HiveConfigException(string message) : base(message) { }

		public HiveConfigException(string message, Exception inner) : base(message, inner) { }
	}
}
