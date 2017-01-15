using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using System.Threading.Tasks;
using Hive.DependencyInjection;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.Web.Rest.RouteHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Hive.Web.Rest
{
	public class RestRouteBuilder
	{
		private readonly IApplicationBuilder _applicationBuilder;
		private readonly IServiceProvider _serviceprovider;
		private readonly IMetaService _metaService;
		private readonly string _prefix;

		public RestRouteBuilder(
			IApplicationBuilder applicationBuilder,
			IServiceProvider serviceprovider,
			IMetaService metaService,
			string prefix)
		{
			_applicationBuilder = applicationBuilder.NotNull(nameof(applicationBuilder));
			_serviceprovider = serviceprovider.NotNull(nameof(serviceprovider));
			_metaService = metaService.NotNull(nameof(metaService));
			_prefix = prefix;
		}

		public IRouter Build()
		{
			var builder = new RouteBuilder(_applicationBuilder);

			var handlerInfos = _metaService.GetHandlerInfos();
			foreach (var handlerInfo in handlerInfos)
			{
				switch (handlerInfo.HandlerType)
				{
					case HandlerTypes.Get:
						var routeHandlerType = typeof(GetRouteHandler<>)
							.GetTypeInfo()
							.MakeGenericType(handlerInfo.ResourceType);

						var routeHandler = (RouteHandlers.IRouteHandler) Activator.CreateInstance(routeHandlerType, _serviceprovider, handlerInfo);
						builder.MapGet(
							$"{_prefix}/{handlerInfo.ResourceDescription.PluralName}/{{{RestConstants.RouteData.Id}{GetTypeConstraint(handlerInfo.KnownIdType)}}}",
							context => routeHandler.Handle(context));
						break;
					case HandlerTypes.Create:
						builder.MapPost(
							$"{_prefix}/{handlerInfo.ResourceDescription.PluralName}",
							context =>
							{
								return Task.CompletedTask;
							}
						);
						break;
					case HandlerTypes.Update:
						builder.MapPut(
							$"{_prefix}/{handlerInfo.ResourceDescription.PluralName}/{{{RestConstants.RouteData.Id}{GetTypeConstraint(handlerInfo.KnownIdType)}}}",
							context =>
							{
								return Task.CompletedTask;
							}
						);
						break;
					case HandlerTypes.Delete:
						builder.MapDelete(
							$"{_prefix}/{handlerInfo.ResourceDescription.PluralName}/{{{RestConstants.RouteData.Id}{GetTypeConstraint(handlerInfo.KnownIdType)}}}",
							context =>
							{
								return Task.CompletedTask;
							}
						);
						break;
					default:
						throw new NotSupportedException($"Unknown handler type {handlerInfo.HandlerType}.");
				}
			}

			return builder.Build();
		}

		// See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/routing#route-constraint-reference
		private static readonly IImmutableDictionary<Type, string> TypeConstraints = new Dictionary<Type, string>
		{
			{ typeof(int), ":int" },
			{ typeof(bool), ":bool" },
			{ typeof(DateTime), ":datetime" },
			{ typeof(decimal), ":decimal" },
			{ typeof(double), ":double" },
			{ typeof(float), ":float" },
			{ typeof(Guid), ":guid" },
			{ typeof(long), ":long" }
		}.ToImmutableDictionary();

		private string GetTypeConstraint(Type type)
		{
			if (type == null)
				return null;

			return TypeConstraints.ContainsKey(type) ? TypeConstraints[type] : null;
		}
	}
}