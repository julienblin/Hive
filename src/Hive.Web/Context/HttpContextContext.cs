using Hive.Context;

namespace Hive.Web.Context
{
	internal class HttpContextContext : IContext
	{
		public string OperationId { get; set; }

		public ClientApplication ClientApplication { get; set; }

		public override string ToString() => $"{ClientApplication} ({OperationId})";
	}
}