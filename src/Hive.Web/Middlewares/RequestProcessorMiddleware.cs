using System.Threading.Tasks;
using Hive.Foundation.Extensions;
using Hive.Web.RequestProcessors;
using Microsoft.AspNetCore.Http;

namespace Hive.Web.Middlewares
{
	public class RequestProcessorMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IRequestProcessor _requestProcessor;

		public RequestProcessorMiddleware(RequestDelegate next, IRequestProcessor requestProcessor)
		{
			_next = next.NotNull(nameof(next));
			_requestProcessor = requestProcessor.NotNull(nameof(requestProcessor));
		}

		public async Task Invoke(HttpContext context)
		{
			context.NotNull(nameof(context));
			
			if (!await _requestProcessor.Process(context, context.RequestAborted))
			{
				await _next.Invoke(context);
			}
		}
	}
}