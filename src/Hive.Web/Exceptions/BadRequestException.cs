using System;
using Hive.Exceptions;

namespace Hive.Web.Exceptions
{
	public class BadRequestException : HiveFatalException
	{
		public BadRequestException(string message) : base(message)
		{
		}

		public BadRequestException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}