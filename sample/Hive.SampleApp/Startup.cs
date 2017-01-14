using System;
using Hive.DependencyInjection;
using Hive.Meta;
using Hive.Meta.Impl;
using Hive.SampleApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Hive.Web;

namespace Hive.SampleApp
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			HostingEnvironment = env;
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json");
			Configuration = builder.Build();
		}

		private IConfigurationRoot Configuration { get; }

		private IHostingEnvironment HostingEnvironment { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddResponseCompression(options => options.Providers.Add<GzipCompressionProvider>());
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

			services.AddOptions();
			services.AddRouting();
			services.AddHive();

			services.AddDefaultHandlers<OS>();
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseResponseCompression();
			app.UseRestRoutes("api");
		}
	}
}