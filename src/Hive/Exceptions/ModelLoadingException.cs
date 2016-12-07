using System;

namespace Hive.Exceptions
{
	public class ModelLoadingException : HiveFatalException
	{
		public ModelLoadingException(string message) : base(message)
		{
		}

		public ModelLoadingException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}