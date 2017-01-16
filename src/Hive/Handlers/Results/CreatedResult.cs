using Hive.Foundation.Extensions;

namespace Hive.Handlers.Results
{
	public class CreatedResult : IHandlerResult
	{
		public CreatedResult(object resource)
		{
			Resource = resource;
		}

		public object Resource { get; set; }
	}
}