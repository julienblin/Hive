using System;

namespace Hive.Exceptions
{
	public class EntityException : HiveFatalException
	{
		public EntityException(string message) : base(message)
		{
		}

		public EntityException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}