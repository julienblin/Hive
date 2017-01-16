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
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;

namespace Hive.Web.Rest
{
	public class RestRouteBuilder
	{
		private readonly IApplicationBuilder _applicationBuilder;
		private readonly IServiceProvider _serviceprovider;
		private readonly IMetaService _metaService;
		private readonly IEnumerable<IInputFormatter> _inputFormatters;
		private readonly IHttpRequestStreamReaderFactory _readerFactory;
		private readonly IModelMetadataProvider _modelMetadataProvider;
		private readonly ICompositeMetadataDetailsProvider _compositeMetadataDetailsProvider;
		private readonly string _prefix;

		public RestRouteBuilder(
			IApplicationBuilder applicationBuilder,
			IServiceProvider serviceprovider,
			IMetaService metaService,
			IEnumerable<IInputFormatter> inputFormatters,
			IHttpRequestStreamReaderFactory readerFactory,
			IModelMetadataProvider modelMetadataProvider,
			ICompositeMetadataDetailsProvider compositeMetadataDetailsProvider,
			string prefix)
		{
			_applicationBuilder = applicationBuilder.NotNull(nameof(applicationBuilder));
			_serviceprovider = serviceprovider.NotNull(nameof(serviceprovider));
			_metaService = metaService.NotNull(nameof(metaService));
			_inputFormatters = inputFormatters.NotNull(nameof(inputFormatters));
			_readerFactory = readerFactory.NotNull(nameof(readerFactory));
			_modelMetadataProvider = modelMetadataProvider.NotNull(nameof(modelMetadataProvider));
			_compositeMetadataDetailsProvider = compositeMetadataDetailsProvider.NotNull(nameof(compositeMetadataDetailsProvider));
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
						var getRouteHandlerType = typeof(GetRouteHandler<,>)
							.GetTypeInfo()
							.MakeGenericType(handlerInfo.ResourceType, handlerInfo.KnownIdType);

						var getRouteHandler = (RouteHandlers.IRouteHandler) Activator.CreateInstance(getRouteHandlerType, _serviceprovider, handlerInfo);
						builder.MapGet(
							$"{_prefix}/{handlerInfo.ResourceDescription.PluralName}/{{{RestConstants.RouteData.Id}{GetTypeConstraint(handlerInfo.KnownIdType)}}}",
							context => getRouteHandler.Handle(context));
						break;
					case HandlerTypes.Create:
						var createRouteHandlerType = typeof(CreateRouteHandler<>)
							.GetTypeInfo()
							.MakeGenericType(handlerInfo.ResourceType);
						var createRouteHandler = (RouteHandlers.IRouteHandler)
							Activator.CreateInstance(createRouteHandlerType, _serviceprovider, _inputFormatters, _readerFactory, handlerInfo, GetModelMetadata(handlerInfo.ResourceType));
						builder.MapPost(
							$"{_prefix}/{handlerInfo.ResourceDescription.PluralName}",
							context => createRouteHandler.Handle(context)
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

		private ModelMetadata GetModelMetadata(Type resourceType)
		{

			return new DefaultModelMetadata(
				_modelMetadataProvider,
				_compositeMetadataDetailsProvider,
				new DefaultMetadataDetails(ModelMetadataIdentity.ForType(resourceType), ModelAttributes.GetAttributesForType(resourceType))
			);
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