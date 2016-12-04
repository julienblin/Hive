using System;
using System.Threading;
using Hive.Cache;
using Hive.Cache.Impl;
using Hive.Config;
using Hive.Entities;
using Hive.Entities.Impl;
using Hive.Foundation.Lifecycle;
using Hive.Meta;
using Hive.Meta.Data;
using Hive.Meta.Data.Impl;
using Hive.Meta.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hive.Foundation.Extensions;
using Hive.Telemetry;
using Hive.Web.Middlewares;
using Hive.Web.Rest;
using Hive.Web.Rest.Serializers;
using Hive.Web.Rest.Serializers.Impl;

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
			services.AddOptions();
			services.Configure<HiveOptions>(Configuration.GetSection("hive"));
			services.Configure<JsonStructureMetaRepositoryOptions>(Configuration.GetSection("meta"));
			services.Configure<RestOptions>(Configuration.GetSection("rest"));

			services.AddSingleton<ITelemetry, DebugTelemetry>();
			services.AddSingleton<IMetaRepository, JsonStructureMetaRepository>();
			services.AddSingleton<IValueTypeFactory, ValueTypeFactory>();
			services.AddSingleton<IModelLoader, ModelLoader>();
			services.AddSingleton<IModelCache, InMemoryModelCache>();
			services.AddSingleton<IMetaService, MetaService>();
			services.AddSingleton<IEntityService, EntityService>();
			services.AddSingleton<IRestSerializerFactory, RestSerializerFactory>();
			services.AddSingleton<RestRequestProcessor>();
		}

		public void Configure(
			IApplicationBuilder app,
			IHostingEnvironment env,
			IServiceProvider serviceProvider)
		{
			serviceProvider.GetServices<IStartable>().SafeForEachParallel((x, ct) => x.Start(ct), CancellationToken.None).Wait();

			app.UseRequestProcessor(serviceProvider.GetRequiredService<RestRequestProcessor>());
		}
	}
}