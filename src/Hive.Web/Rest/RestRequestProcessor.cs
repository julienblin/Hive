using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
using Hive.Web.Extensions;
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
			if (pathSegments == null || pathSegments.Length < 2) return false;

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

			if (exception is MissingETagException)
			{
				context.Response.StatusCode = StatusCodes.Status400BadRequest;

				context.Response.Headers["Content-Type"] = responseSerializer.MediaTypes.First();
				responseSerializer.Serialize(new MessageResponse(exception.Message), context.Response.Body);
				return Task.CompletedTask;
			}

			if (exception is ConcurrencyException)
			{
				context.Response.StatusCode = StatusCodes.Status409Conflict;

				context.Response.Headers["Content-Type"] = responseSerializer.MediaTypes.First();
				responseSerializer.Serialize(new MessageResponse(exception.Message), context.Response.Body);
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

				var ifNoneMatch = param.Headers.IfNoneMatch();
				if (string.Equals(ifNoneMatch, result.Etag, StringComparison.Ordinal))
				{
					Respond(param, null, StatusCodes.Status304NotModified, new Dictionary<string, string>
					{
						{ WebConstants.ETagHeader, result.Etag }
					});
				}
				else
				{
					Respond(param, result.ToPropertyBag(), StatusCodes.Status200OK, new Dictionary<string, string>
					{
						{ WebConstants.ETagHeader, result.Etag }
					});
				}
			}
			else
			{
				var query = CreateQuery(entityDefinition, restQuery, param);
				if (query.MaxResults.HasValue)
				{
					var result = await query.ToContinuationEnumerable<IEntity>(ct);
					Respond(param, result.Select(x => x.ToPropertyBag()).ToArray(), StatusCodes.Status200OK, new Dictionary<string, string>
					{
						{ RestConstants.ContinuationTokenHeader, result.ContinuationToken }
					});
				}
				else
				{
					var result = await query.ToEnumerable<IEntity>(ct);
					Respond(param, result.Select(x => x.ToPropertyBag()).ToArray(), StatusCodes.Status200OK);
				}
			}
		}

		private IQuery CreateQuery(IEntityDefinition entityDefinition, RestQueryString restQuery, RestProcessParameters param)
		{
			var query = EntityService.CreateQuery(entityDefinition);
			foreach (var queryStringValue in restQuery.QueryStringValues.Where(x => !x.Key.StartsWith(RestConstants.ReservedOperatorsPrefix)))
			{
				RecursiveFillQuery(query, null, queryStringValue.Key, queryStringValue.Value);
			}

			FillOperators(query, restQuery, param);

			if (restQuery.PathValues.Count == 0)
				return query;

			if (restQuery.PathValues.Count > 1)
				throw new BadRequestException("Multi-level query expressions in path is not supported.");

			var pathValue = restQuery.PathValues.First();

			var targetEntityDef = entityDefinition.Model.EntitiesByPluralName.SafeGet(pathValue.Key);
			if (targetEntityDef == null)
				throw new NotFoundException($"Unable to find an entity definition named {pathValue.Key}");

			var targetPropertyDefinition = entityDefinition.Properties.SafeGet(targetEntityDef.SingleName);
			if (targetPropertyDefinition != null)
			{
				var subQuery = query.GetOrCreateSubQuery(targetPropertyDefinition.Name);
				RecursiveFillQuery(subQuery, null, MetaConstants.IdProperty, pathValue.Value);
			}

			return query;
		}

		private static readonly Regex OrderByRegex = new Regex(@"^(?<prop>[^\s]+)\s*(?<asc>asc|desc)?$", RegexOptions.Compiled);

		private static void FillOperators(IQuery query, RestQueryString restQuery, RestProcessParameters param)
		{
			if (restQuery.QueryStringValues.ContainsKey(RestConstants.LimitOperator))
			{
				var maxResults = restQuery.QueryStringValues[RestConstants.LimitOperator].First().IntSafeInvariantParse();
				query.SetMaxResults(maxResults);
			}

			if (restQuery.QueryStringValues.ContainsKey(RestConstants.OrderOperator))
			{
				var orderby = restQuery.QueryStringValues[RestConstants.OrderOperator].FirstOrDefault();
				var orderbyMatch = OrderByRegex.Match(orderby);
				if (orderbyMatch.Success)
				{
					switch (orderbyMatch.Groups["asc"].Value.IsNullOrEmpty() ? "asc" : orderbyMatch.Groups["asc"].Value)
					{
						case "asc":
							query.AddOrder(Order.Asc(orderbyMatch.Groups["prop"].Value));
							break;
						case "desc":
							query.AddOrder(Order.Desc(orderbyMatch.Groups["prop"].Value));
							break;
					}
				}
			}

			if (restQuery.QueryStringValues.ContainsKey(RestConstants.IncludeOperator))
			{
				foreach (var value in restQuery.QueryStringValues[RestConstants.IncludeOperator])
				{
					query.Include(value);
				}
			}

			if (restQuery.QueryStringValues.ContainsKey(RestConstants.SelectOperator))
			{
				query.SetProjection(Projection.Properties(restQuery.QueryStringValues[RestConstants.SelectOperator]));
			}

			if (param.Context.Request.Headers.ContainsKey(RestConstants.ContinuationTokenHeader))
			{
				var continuationToken = param.Context.Request.Headers[RestConstants.ContinuationTokenHeader].FirstOrDefault();
				query.SetContinuationToken(continuationToken);
			}
		}

		private static void RecursiveFillQuery(IQuery query, string protectedLeading, string selector, StringValues selectorValues)
		{
			string remaining;
			var leading = selector.SplitFirst('.', out remaining);
			if ((remaining == null) || remaining.SafeOrdinalEquals(MetaConstants.IdProperty))
			{
				AddCriterion(query, protectedLeading == null ? selector : $"{protectedLeading}.{selector}", selectorValues);
				return;
			}

			var propertyDefinition = query.EntityDefinition.Properties.SafeGet(leading);
			if(propertyDefinition == null)
				throw new BadRequestException($"Unable to find a property named {leading} on {query.EntityDefinition}");

			if (propertyDefinition.PropertyType.DataTypeType == DataTypeType.Relation)
			{
				var subQuery = query.GetOrCreateSubQuery(leading);
				RecursiveFillQuery(subQuery, null, remaining, selectorValues);
			}
			else
			{
				RecursiveFillQuery(query, leading, remaining, selectorValues);
			}
		}

		private static void AddCriterion(IQuery query, string selector, StringValues selectorValues)
		{
			if (selectorValues.Count > 1)
				query.Add(Criterion.In(selector, selectorValues));
			else
				query.Add(Criterion.Eq(selector, selectorValues.FirstOrDefault()));
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
				{ WebConstants.ETagHeader, entity.Etag },
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
			entity.Etag = param.Headers.IfNoneMatch();

			var cmd = new UpdateCommand(entity);
			var result = await EntityService.Execute(cmd, ct);

			Respond(param, result.ToPropertyBag(), StatusCodes.Status200OK, new Dictionary<string, string>
			{
				{ WebConstants.ETagHeader, entity.Etag }
			});
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

			if (message != null)
			{
				param.Context.Response.Headers["Content-Type"] = param.ResponseSerializer.MediaTypes.First();
				param.ResponseSerializer.Serialize(message, param.Context.Response.Body);
			}
		}
	}
}