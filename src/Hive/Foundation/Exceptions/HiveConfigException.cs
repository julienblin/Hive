using System;

namespace Hive.Foundation.Exceptions
{
	public class HiveConfigException : HiveFatalException
	{
		public HiveConfigException(string message) : base(message) { }

		public HiveConfigException(string message, Exception inner) : base(message, inner) { }
	}
}
