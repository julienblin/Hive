using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.Telemetry;
using Microsoft.AspNetCore.Http;

namespace Hive.Web.RequestProcessors
{
	public abstract class RequestProcessor : IRequestProcessor
	{
		protected RequestProcessor(
			ITelemetry telemetry,
			IMetaService metaService,
			IEntityService entityService)
		{
			Telemetry = telemetry.NotNull(nameof(telemetry));
			MetaService = metaService.NotNull(nameof(metaService));
			EntityService = entityService.NotNull(nameof(entityService));
		}

		protected ITelemetry Telemetry { get; }

		protected IMetaService MetaService { get; }

		protected IEntityService EntityService { get; }

		public virtual async Task<bool> Process(HttpContext context, CancellationToken ct)
		{
			try
			{
				return await ProcessInternal(context, ct);
			}
			catch (Exception ex)
			{
				Telemetry.TrackException(ex, new Dictionary<string, string>
				{
					{"Method", context.Request.Method},
					{"Path", context.Request.Path},
					{"QueryString", context.Request.QueryString.ToString()}
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
	}
}