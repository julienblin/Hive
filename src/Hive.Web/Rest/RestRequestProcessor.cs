﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hive.Commands;
using Hive.Context;
using Hive.Entities;
using Hive.Exceptions;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.Queries;
using Hive.Telemetry;
using Hive.Web.Exceptions;
using Hive.Web.RequestProcessors;
using Hive.Web.Rest.Responses;
using Hive.Web.Rest.Serializers;
using Microsoft.AspNetCore.Http;
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
			IContextService contextService,
			IMetaService metaService,
			IEntityFactory entityfactory,
			IEntityService entityService,
			IRestSerializerFactory serializerFactory)
			: base(telemetry, contextService, metaService, entityfactory, entityService)
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
			var requestSerializer = _serializerFactory.GetByMediaType(headers.ContentType?.MediaType);
			var responseSerializer = headers.Accept.IsNullOrEmpty()
				? requestSerializer
				: _serializerFactory.GetByMediaType(headers.Accept.Safe().Select(x => x.MediaType));
			var processParams = new RestProcessParameters(context, headers, pathSegments, model, requestSerializer,
				responseSerializer);

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

			if (HttpMethods.IsPut(request.Method))
			{
				await ProcessPutCommand(processParams, ct);
				return true;
			}

			if (HttpMethods.IsDelete(request.Method))
			{
				await ProcessDeleteCommand(processParams, ct);
				return true;
			}

			throw new NotSupportedException();
		}

		protected override Task ProcessException(HttpContext context, Exception exception, CancellationToken ct)
		{
			var headers = context.Request.GetTypedHeaders();
			var responseSerializer = headers.Accept.IsNullOrEmpty()
				? _serializerFactory.GetByMediaType(headers.ContentType?.MediaType)
				: _serializerFactory.GetByMediaType(headers.Accept.Safe().Select(x => x.MediaType));

			if (exception is BadRequestException)
			{
				var badRequestException = (BadRequestException)exception;
				context.Response.StatusCode = StatusCodes.Status400BadRequest;

				context.Response.Headers["Content-Type"] = responseSerializer.MediaTypes.First();
				responseSerializer.Serialize(new MessageResponse(badRequestException.Message), context.Response.Body);
				return Task.CompletedTask;
			}

			if (exception is QueryException)
			{
				var queryException = (QueryException)exception;
				context.Response.StatusCode = StatusCodes.Status400BadRequest;

				context.Response.Headers["Content-Type"] = responseSerializer.MediaTypes.First();
				responseSerializer.Serialize(new MessageResponse(queryException.Message), context.Response.Body);
				return Task.CompletedTask;
			}

			if (exception is NotFoundException)
			{
				var notFoundException = (NotFoundException)exception;
				context.Response.StatusCode = StatusCodes.Status404NotFound;

				context.Response.Headers["Content-Type"] = responseSerializer.MediaTypes.First();
				responseSerializer.Serialize(new MessageResponse(notFoundException.Message), context.Response.Body);
				return Task.CompletedTask;
			}

			if (exception is ValidationException)
			{
				var validationException = (ValidationException)exception;
				context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

				context.Response.Headers["Content-Type"] = responseSerializer.MediaTypes.First();
				responseSerializer.Serialize(validationException.Results, context.Response.Body);
				return Task.CompletedTask;
			}

			return base.ProcessException(context, exception, ct);
		}

		private async Task ProcessQuery(RestProcessParameters param, CancellationToken ct)
		{
			var restQuery = new RestQueryString(param);
			var entityDefinition = param.Model.EntitiesByPluralName.SafeGet(restQuery.Root);
			if (entityDefinition == null)
				throw new NotFoundException($"Unable to find an entity definition named {restQuery.Root}.");

			if (!restQuery.AdditionalQualifier.IsNullOrEmpty())
			{
				var result = await EntityService.GetById(entityDefinition, restQuery.AdditionalQualifier, ct);
				if (result == null)
				{
					Respond(param,
						new MessageResponse($"Unable to find a {entityDefinition.SingleName} with id {restQuery.AdditionalQualifier}"),
						StatusCodes.Status404NotFound);
					return;
				}

				Respond(param, result.ToPropertyBag(), StatusCodes.Status200OK);
				return;
			}
			else
			{
				var query = CreateQuery(entityDefinition, restQuery);
				var result = await query.ToEnumerable<IEntity>(ct);
				Respond(param, result.Select(x => x.ToPropertyBag()).ToArray(), StatusCodes.Status200OK);
				return;
			}
		}

		private IQuery CreateQuery(IEntityDefinition entityDefinition, RestQueryString restQuery)
		{
			var query = EntityService.CreateQuery(entityDefinition);
			foreach (var queryStringValue in restQuery.QueryStringValues.Where(x => !x.Key.StartsWith("$") && !x.Key.Contains(".")))
			{
				AddQueryStringCriterion(query, queryStringValue);
			}

			foreach (var queryStringValue in restQuery.QueryStringValues.Where(x => !x.Key.StartsWith("$") && x.Key.Contains(".")))
			{
				var relationSplit = queryStringValue.Key.Split('.');
				if (relationSplit.Length != 2)
					throw new BadRequestException("Multiple relation-level queries are not supported.");

				var subQuery = query.GetOrCreateSubQuery(relationSplit[0]);
				AddQueryStringCriterion(subQuery, new KeyValuePair<string, StringValues>(relationSplit[1], queryStringValue.Value));
			}

			foreach (var pathValue in restQuery.PathValues)
			{
				var targetEntityDef = entityDefinition.Model.EntitiesByPluralName.SafeGet(pathValue.Key);
				if(targetEntityDef == null)
					throw new NotFoundException($"Unable to find an entity definition named {pathValue.Key}");

				// First, let's find by single name
				var targetPropertyDefinition = entityDefinition.Properties.SafeGet(targetEntityDef.SingleName);
				if (targetPropertyDefinition != null)
				{
					var subQuery = query.GetOrCreateSubQuery(targetPropertyDefinition.Name);
					AddQueryStringCriterion(subQuery, new KeyValuePair<string, StringValues>(MetaConstants.IdProperty, pathValue.Value));
				}
			}

			return query;
		}

		private static void AddQueryStringCriterion(IQuery query, KeyValuePair<string, StringValues> queryStringValue)
		{
			if (queryStringValue.Value.Count > 1)
				query.Add(Criterion.In(queryStringValue.Key, queryStringValue.Value));
			else
				query.Add(Criterion.Eq(queryStringValue.Key, queryStringValue.Value.FirstOrDefault()));
		}

		private async Task ProcessPostCommand(RestProcessParameters param, CancellationToken ct)
		{
			var restQuery = new RestQueryString(param);
			var entityDefinition = param.Model.EntitiesByPluralName.SafeGet(restQuery.Root);
			if (entityDefinition == null)
				throw new BadRequestException($"Unable to find an entity definition named {restQuery.Root}.");

			if (!restQuery.AdditionalQualifier.IsNullOrEmpty())
				throw new BadRequestException($"A post request should not have an additional qualifier.");

			var propertyBag = param.RequestSerializer.Deserialize(param.Context.Request.Body);
			var entity = await EntityFactory.Create(entityDefinition, propertyBag, ct);

			var cmd = new CreateCommand(entity);
			var result = await EntityService.Execute(cmd, ct);

			Respond(param, result.ToPropertyBag(), StatusCodes.Status201Created, new Dictionary<string, string>
			{
				{"Location", $"{_options.Value.MountPoint}/{entityDefinition.Model.Name}/{entityDefinition.PluralName}/{result.Id}"}
			});
		}

		private async Task ProcessPutCommand(RestProcessParameters param, CancellationToken ct)
		{
			var restQuery = new RestQueryString(param);
			var entityDefinition = param.Model.EntitiesByPluralName.SafeGet(restQuery.Root);
			if (entityDefinition == null)
				throw new BadRequestException($"Unable to find an entity definition named {restQuery.Root}.");

			if (restQuery.AdditionalQualifier.IsNullOrEmpty())
				throw new BadRequestException($"A put request must apply to a specific id.");

			var propertyBag = param.RequestSerializer.Deserialize(param.Context.Request.Body);
			propertyBag[MetaConstants.IdProperty] = restQuery.AdditionalQualifier;
			var entity = await EntityFactory.Hydrate(entityDefinition, propertyBag, ct);

			var cmd = new UpdateCommand(entity);
			var result = await EntityService.Execute(cmd, ct);

			Respond(param, result.ToPropertyBag(), StatusCodes.Status200OK);
		}

		private async Task ProcessDeleteCommand(RestProcessParameters param, CancellationToken ct)
		{
			var restQuery = new RestQueryString(param);
			var entityDefinition = param.Model.EntitiesByPluralName.SafeGet(restQuery.Root);
			if (entityDefinition == null)
				throw new BadRequestException($"Unable to find an entity definition named {restQuery.Root}.");

			if (restQuery.AdditionalQualifier.IsNullOrEmpty())
				throw new BadRequestException($"A delete request must include an id.");

			var cmd = new DeleteCommand(entityDefinition, restQuery.AdditionalQualifier);
			var result = await EntityService.Execute(cmd, ct);
			if (!result)
			{
				Respond(param,
					new MessageResponse($"Unable to find a {entityDefinition.SingleName} with id {restQuery.AdditionalQualifier}."),
					StatusCodes.Status404NotFound);
				return;
			}
			Respond(param, null, StatusCodes.Status204NoContent);
		}

		private void Respond(RestProcessParameters param, object message, int statusCode,
			IDictionary<string, string> responseHeaders = null)
		{
			param.Context.Response.StatusCode = statusCode;
			foreach (var responseHeader in responseHeaders.Safe())
				param.Context.Response.Headers.Add(responseHeader.Key, new StringValues(responseHeader.Value));

			param.Context.Response.Headers["Content-Type"] = param.ResponseSerializer.MediaTypes.First();
			if (message != null)
				param.ResponseSerializer.Serialize(message, param.Context.Response.Body);
		}
	}
}