using System;
using System.Threading.Tasks;
using Hive.DependencyInjection;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Hive.Web.Rest
{
	public class RestRouteBuilder
	{
		private readonly IApplicationBuilder _applicationBuilder;
		private readonly IMetaService _metaService;
		private readonly string _prefix;

		public RestRouteBuilder(
			IApplicationBuilder applicationBuilder,
			IMetaService metaService,
			string prefix)
		{
			_applicationBuilder = applicationBuilder.NotNull(nameof(applicationBuilder));
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
						builder.MapGet(
							$"{_prefix}/{handlerInfo.ResourceDescription.PluralName}/{{id}}",
							context =>
							{
								return Task.CompletedTask;
							}
						);
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
							$"{_prefix}/{handlerInfo.ResourceDescription.PluralName}/{{id}}",
							context =>
							{
								return Task.CompletedTask;
							}
						);
						break;
					case HandlerTypes.Delete:
						builder.MapDelete(
							$"{_prefix}/{handlerInfo.ResourceDescription.PluralName}/{{id}}",
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
	}
}