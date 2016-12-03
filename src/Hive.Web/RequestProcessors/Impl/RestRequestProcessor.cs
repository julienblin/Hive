using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hive.Config;
using Hive.Entities;
using Hive.Foundation.Exceptions;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.Queries;
using Hive.Serialization;
using Hive.Telemetry;
using Microsoft.AspNetCore.Http;

namespace Hive.Web.RequestProcessors.Impl
{
	public class RestRequestProcessor : RequestProcessor
	{
		public RestRequestProcessor(
			IHiveConfig config,
			ITelemetry telemetry,
			IMetaService metaService,
			IEntityService entityService,
			IEntitySerializerFactory entitySerializerFactory,
			PathString mountPoint)
			: base(config, telemetry, metaService, entityService, entitySerializerFactory, mountPoint)
		{
		}

		protected override async Task<bool> ProcessInternal(HttpContext context, CancellationToken ct)
		{
			var request = context.Request;

			PathString partialPath;
			if (!request.Path.StartsWithSegments(MountPoint, out partialPath)) return false;

			var pathSegments = partialPath.Value?.Split('/').Where(x => !x.Trim().IsNullOrEmpty()).ToArray();
			if (pathSegments?.Length < 2) return false;

			var model = await MetaService.GetModel(pathSegments[0], ct);
			pathSegments = pathSegments.Skip(1).ToArray();

			if (HttpMethods.IsGet(request.Method))
			{
				await ProcessQuery(context, pathSegments, model, ct);
				return true;
			}

			if (HttpMethods.IsPost(request.Method)
			    || HttpMethods.IsPut(request.Method)
			    || HttpMethods.IsPatch(request.Method)
			    || HttpMethods.IsDelete(request.Method)
			)
			{
				await ProcessCommand(context, pathSegments, model, ct);
				return true;
			}

			throw new NotSupportedException();
		}

		private async Task ProcessQuery(HttpContext context, string[] pathSegments, IModel model, CancellationToken ct)
		{
			var query = BuildQuery(context.Request, pathSegments, model);
			var result = await EntityService.Execute(query, ct);
			if (query is IdQuery)
			{
				var firstResult = result.FirstOrDefault();
				if (firstResult == null)
				{
					throw new NotFoundException($"Unable to find a {query.ResultType} with Id {((IdQuery)query).Id}.");
				}

				// Process response;
			}

			// Process response;
		}

		private Query BuildQuery(HttpRequest request, string[] pathSegments, IModel model)
		{
			var query = new Query(new Context.Context());
			return query;
		}

		private Task ProcessCommand(HttpContext context, string[] pathSegments, IModel model, CancellationToken ct)
		{
			throw new NotImplementedException();
		}
	}
}