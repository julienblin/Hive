using System;
using Hive.Azure.DocumentDb;
using Hive.Data;
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
using Microsoft.AspNetCore.Mvc.Formatters;

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
			services.Configure<DocumentDbOptions>(Configuration.GetSection("documentdb"));

			services.AddSingleton<IEntityRepository, DocumentDbEntityRepository>();
			services.AddHive();

			services.AddMvc();

			services.AddEntityRepositoryHandlers<OS, Guid>();
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseResponseCompression();
			app.UseRestRoutes("api");
			app.UseMvcWithDefaultRoute();
		}
	}
}