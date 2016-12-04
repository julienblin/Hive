using System;
using System.IO;
using System.Threading;
using Hive.Cache;
using Hive.Cache.Impl;
using Hive.Config;
using Hive.Config.Impl;
using Hive.Entities;
using Hive.Entities.Impl;
using Hive.Foundation.Lifecycle;
using Hive.Meta;
using Hive.Meta.Data;
using Hive.Meta.Data.Impl;
using Hive.Meta.Impl;
using Hive.Serialization;
using Hive.Serialization.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Hive.Foundation.Extensions;
using Hive.Telemetry;
using Hive.Web.Middlewares;
using Hive.Web.Rest;

namespace Hive.SampleApp
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			HostingEnvironment = env;
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath);
			Configuration = builder.Build();
		}

		private IConfigurationRoot Configuration { get; }

		private IHostingEnvironment HostingEnvironment { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			var configService = new JsonConfigService(Path.Combine(HostingEnvironment.ContentRootPath, "hiveconfig.json"));
			services.AddSingleton<IConfigService>(x => configService);
			services.AddSingleton<ITelemetry, DebugTelemetry>();
			services.AddSingleton<IMetaRepository, JsonStructureMetaRepository>();
			services.AddSingleton<IValueTypeFactory>(x => new ValueTypeFactory());
			services.AddSingleton<IModelLoader, ModelLoader>();
			services.AddSingleton<IModelCache, InMemoryModelCache>();
			services.AddSingleton<IMetaService, MetaService>();
			services.AddSingleton<IEntityService, EntityService>();
			services.AddSingleton<IEntitySerializerFactory, EntitySerializerFactory>();
			services.AddSingleton(x => new RestRequestProcessor(
				configService.GetConfig(CancellationToken.None).GetAwaiter().GetResult(),
				x.GetRequiredService<ITelemetry>(),
				x.GetRequiredService<IMetaService>(),
				x.GetRequiredService<IEntityService>(),
				x.GetRequiredService<IEntitySerializerFactory>(),
				"/api")
			);
		}

		public void Configure(
			IApplicationBuilder app,
			IHostingEnvironment env,
			ILoggerFactory loggerFactory,
			IServiceProvider serviceProvider)
		{
			serviceProvider.GetServices<IStartable>().SafeForEachParallel((x, ct) => x.Start(ct), CancellationToken.None).Wait();

			app.UseRequestProcessor(serviceProvider.GetRequiredService<RestRequestProcessor>());
		}
	}
}