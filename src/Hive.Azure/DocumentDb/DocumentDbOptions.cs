using System;

namespace Hive.Azure.DocumentDb
{
	public class DocumentDbOptions
	{
		public Uri ServiceEndpoint { get; set; }

		public string AuthKey { get; set; }

		public string Database { get; set; }

		public string Collection { get; set; }
	}
}