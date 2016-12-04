using System;

namespace Hive.Exceptions
{
	public class SerializationException : HiveFatalException
	{
		public SerializationException(string message) : base(message)
		{
		}

		public SerializationException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}