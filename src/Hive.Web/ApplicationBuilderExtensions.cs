using System.Collections.Generic;
using Hive.Meta;
using Hive.Web.Rest;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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
				app.ApplicationServices.GetService<IOptions<MvcOptions>>().Value.InputFormatters,
				app.ApplicationServices.GetRequiredService<IHttpRequestStreamReaderFactory>(),
				app.ApplicationServices.GetRequiredService<IModelMetadataProvider>(),
				app.ApplicationServices.GetRequiredService<ICompositeMetadataDetailsProvider>(),
				prefix
			);

			app.UseRouter(restRouteBuilder.Build());

			return app;
		}
	}
}