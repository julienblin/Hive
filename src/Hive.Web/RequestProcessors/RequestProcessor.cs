using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Config;
using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.Serialization;
using Hive.Telemetry;
using Microsoft.AspNetCore.Http;

namespace Hive.Web.RequestProcessors
{
	public abstract class RequestProcessor : IRequestProcessor
	{
		private readonly ITelemetry _telemetry;
		private readonly IMetaService _metaService;
		private readonly IEntityService _entityService;
		private readonly IEntitySerializerFactory _entitySerializerFactory;

		protected RequestProcessor(
			ITelemetry telemetry,
			IMetaService metaService,
			IEntityService entityService,
			IEntitySerializerFactory entitySerializerFactory)
		{
			_telemetry = telemetry.NotNull(nameof(telemetry));
			_metaService = metaService.NotNull(nameof(metaService));
			_entityService = entityService.NotNull(nameof(entityService));
			_entitySerializerFactory = entitySerializerFactory.NotNull(nameof(entitySerializerFactory));
		}

		public virtual async Task<bool> Process(HttpContext context, CancellationToken ct)
		{
			try
			{
				return await ProcessInternal(context, ct);
			}
			catch (Exception ex)
			{
				_telemetry.TrackException(ex, new Dictionary<string, string>
				{
					{ "Method", context.Request.Method },
					{ "Path", context.Request.Path },
					{ "QueryString", context.Request.QueryString.ToString() }
				});

				await ProcessException(context, ex, ct);
				return true;
			}
		}

		protected abstract Task<bool> ProcessInternal(HttpContext context, CancellationToken ct);

		protected virtual Task ProcessException(HttpContext context, Exception exception, CancellationToken ct)
		{
			context.Response.StatusCode = 500;
			return context.Response.WriteAsync(exception.ToString(), ct);
		}

		protected IMetaService MetaService => _metaService;

		protected IEntityService EntityService => _entityService;

		protected IEntitySerializerFactory EntitySerializerFactory => _entitySerializerFactory;
	}
}