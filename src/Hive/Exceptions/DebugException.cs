using System;

namespace Hive.Exceptions
{
#if DEBUG
	public class DebugException : HiveFatalException
	{
		public DebugException(string message) : base(message)
		{
		}

		public DebugException(string message, Exception inner) : base(message, inner)
		{
		}
	}
#endif
}