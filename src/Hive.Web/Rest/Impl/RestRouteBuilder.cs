using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hive.Foundation.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Hive.Web.Rest.Impl
{
	public class RestRouteBuilder
	{
		private readonly IApplicationBuilder _applicationBuilder;
		private readonly IServiceCollection _serviceCollection;

		public RestRouteBuilder(IApplicationBuilder applicationBuilder, IServiceCollection serviceCollection)
		{
			_applicationBuilder = applicationBuilder.NotNull(nameof(applicationBuilder));
			_serviceCollection = serviceCollection.NotNull(nameof(serviceCollection));
		}

		public void BuildRoutes()
		{
			var builder = new RouteBuilder(_applicationBuilder);
		}

		public IRouter Build()
		{
			throw new System.NotImplementedException();
		}
	}
}