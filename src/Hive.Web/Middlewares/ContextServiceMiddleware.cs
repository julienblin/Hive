using System.Threading.Tasks;
using Hive.Context;
using Hive.Foundation.Extensions;
using Microsoft.AspNetCore.Http;

namespace Hive.Web.Middlewares
{
	public class ContextServiceMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IContextService _contextService;

		public ContextServiceMiddleware(RequestDelegate next, IContextService contextService)
		{
			_next = next.NotNull(nameof(next));
			_contextService = contextService.NotNull(nameof(contextService));
		}

		public async Task Invoke(HttpContext context)
		{
			context.NotNull(nameof(context));

			var hiveContext =_contextService.StartContext();
			context.Response.Headers[WebConstants.OperationIdHeader] = hiveContext.OperationId;
			await _next.Invoke(context);
			_contextService.StopContext();
		}
	}
}