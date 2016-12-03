using Hive.Web.RequestProcessors;
using Microsoft.AspNetCore.Builder;

namespace Hive.Web.Middlewares
{
	public static class ApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseRequestProcessor(this IApplicationBuilder builder, IRequestProcessor requestProcessor)
		{
			return builder.UseMiddleware<RequestProcessorMiddleware>(requestProcessor);
		}
	}
}