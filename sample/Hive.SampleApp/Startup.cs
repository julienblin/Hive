using System;
using System.Threading;
using Hive.Azure.DocumentDb;
using Hive.Cache;
using Hive.Cache.Impl;
using Hive.Config;
using Hive.Entities;
using Hive.Entities.Impl;
using Hive.Foundation.Extensions;
using Hive.Foundation.Lifecycle;
using Hive.Meta;
using Hive.Meta.Impl;
using Hive.Telemetry;
using Hive.Validation;
using Hive.Validation.Impl;
using Hive.ValueTypes;
using Hive.Web.Middlewares;
using Hive.Web.Rest;
using Hive.Web.Rest.Serializers;
using Hive.Web.Rest.Serializers.Impl;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
			services.Configure<DocumentDbOptions>(Configuration.GetSection("documentdb"));

			if (HostingEnvironment.IsDevelopment())
				services.AddSingleton<IModelCache, NullModelCache>();
			else
				services.AddSingleton<IModelCache, MemoryModelCache>();

			services.AddSingleton<ITelemetry, DebugTelemetry>();
			services.AddSingleton<IMetaRepository, JsonStructureMetaRepository>();
			services.AddSingleton<IValueTypeFactory, ValueTypeFactory>();
			services.AddSingleton<IModelLoader, ModelLoader>();
			services.AddSingleton<IMetaService, MetaService>();
			services.AddSingleton<IEntityFactory, EntityFactory>();
			services.AddSingleton<IEntityRepository, DocumentDbEntityRepository>();
			services.AddSingleton<IEntityValidationService, EntityValidationService>();
			services.AddSingleton<IEntityService, EntityService>();
			services.AddSingleton<IRestSerializerFactory, RestSerializerFactory>();
			services.AddSingleton<RestRequestProcessor>();

			// Have to do it manually for now...
			services.AddSingleton(x => (IStartable) x.GetService<IEntityRepository>());
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