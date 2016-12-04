using Hive.Foundation.Extensions;

namespace Hive.Web.Rest
{
	public class MessageResponse
	{
		public MessageResponse(string message)
		{
			Message = message.NotNullOrEmpty(nameof(message));
		}

		public string Message { get; }
	}
}