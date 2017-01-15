using Hive.Meta;
using Hive.Web.Rest;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Hive.Web
{
	public static class ApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseRestRoutes(this IApplicationBuilder app, string prefix)
		{
			var restRouteBuilder = new RestRouteBuilder(
				app,
				app.ApplicationServices,
				app.ApplicationServices.GetRequiredService<IMetaService>(),
				prefix
			);

			app.UseRouter(restRouteBuilder.Build());

			return app;
		}
	}
}