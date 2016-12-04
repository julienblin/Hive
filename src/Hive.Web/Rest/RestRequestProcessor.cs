using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.Queries;
using Hive.Telemetry;
using Hive.Web.RequestProcessors;
using Hive.Web.Rest.Responses;
using Hive.Web.Rest.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Hive.Web.Rest
{
	public class RestRequestProcessor : RequestProcessor
	{
		private readonly IOptions<RestOptions> _options;
		private readonly IRestSerializerFactory _serializerFactory;

		public RestRequestProcessor(
			IOptions<RestOptions> options,
			ITelemetry telemetry,
			IMetaService metaService,
			IEntityService entityService,
			IRestSerializerFactory serializerFactory)
			: base(telemetry, metaService, entityService)
		{
			_options = options.NotNull(nameof(options));
			_serializerFactory = serializerFactory.NotNull(nameof(serializerFactory));
		}

		protected override async Task<bool> ProcessInternal(HttpContext context, CancellationToken ct)
		{
			var request = context.Request;

			PathString partialPath;
			if (!request.Path.StartsWithSegments(_options.Value.MountPoint, out partialPath)) return false;

			var pathSegments = partialPath.Value?.Split('/').Where(x => !x.Trim().IsNullOrEmpty()).ToArray();
			if (pathSegments?.Length < 2) return false;

			var model = await MetaService.GetModel(pathSegments[0], ct);
			pathSegments = pathSegments.Skip(1).ToArray();

			var headers = context.Request.GetTypedHeaders();
			var serializer = _serializerFactory.GetByMediaType(headers.Accept.Select(x => x.MediaType));
			var processParams = new RestProcessParameters(context, headers, pathSegments, model, serializer);

			if (HttpMethods.IsGet(request.Method))
			{
				await ProcessQuery(processParams, ct);
				return true;
			}

			if (HttpMethods.IsPost(request.Method))
			{
				await ProcessPostCommand(processParams, ct);
				return true;
			}

			throw new NotSupportedException();
		}

		private async Task ProcessQuery(RestProcessParameters param, CancellationToken ct)
		{
			var query = BuildQuery(param);

			if (query is IdQuery)
			{
				var idQuery = (IdQuery)query;
				var result = await EntityService.Execute(idQuery, ct);
				if (result == null)
				{
					Respond(param, new MessageResponse($"Unable to find a {idQuery.EntityDefinition.SingleName} with id {idQuery.Id}"), StatusCodes.Status404NotFound);
					return;
				}

				Respond(param, result, StatusCodes.Status200OK);
				return;
			}

			throw new NotImplementedException();
		}

		private IQuery BuildQuery(RestProcessParameters param)
		{
			var query = new IdQuery(null, null);
			return query;
		}

		private Task ProcessPostCommand(RestProcessParameters processParams, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		private void Respond(RestProcessParameters param, object message, int statusCode, IDictionary<string, string> responseHeaders = null)
		{
			param.Context.Response.StatusCode = statusCode;
			foreach (var responseHeader in responseHeaders.Safe())
			{
				param.Context.Response.Headers.Add(responseHeader.Key, new StringValues(responseHeader.Value));
			}

			param.Serializer.Serialize(message, param.Context.Response.Body);
		}
	}
}