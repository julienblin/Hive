﻿namespace Hive.Web.Rest
{
	public class Error
	{
		public ErrorCode Code { get; set; }

		public string Message { get; set; }

		public string Target { get; set; }

		public Error[] Details { get; set; }
	}
}