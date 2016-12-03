using System;

namespace Hive.Foundation.Exceptions
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