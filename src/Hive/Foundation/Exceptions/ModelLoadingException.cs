using System;

namespace Hive.Foundation.Exceptions
{
	public class ModelLoadingException : HiveFatalException
	{
		public ModelLoadingException(string message) : base(message) { }

		public ModelLoadingException(string message, Exception inner) : base(message, inner) { }

	}
}