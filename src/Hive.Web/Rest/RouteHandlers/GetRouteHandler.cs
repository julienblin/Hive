using System;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Handlers;
using Hive.Meta;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Hive.Web.Rest.RouteHandlers
{
	public class GetRouteHandler<TResource, TId> : RouteHandler
		where TResource : IIdentifiable<TId>
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly HandlerInfo _handlerInfo;

		public GetRouteHandler(IServiceProvider serviceProvider, HandlerInfo handlerInfo)
		{
			_serviceProvider = serviceProvider.NotNull(nameof(serviceProvider));
			_handlerInfo = handlerInfo.NotNull(nameof(handlerInfo));
		}

		public override async Task Handle(HttpContext context)
		{
			var handler = _serviceProvider.GetService(_handlerInfo.HandlerInterfaceType) as IHandleGet<TResource, TId>;
			var id = context.GetRouteValue(RestConstants.RouteData.Id);

			if (_handlerInfo.KnownIdType != null)
			{
				id = ConvertKnownIdType(id, _handlerInfo.KnownIdType);
			}

			var result = await handler.Get((TId)id, context.RequestAborted);
			await InterpretResult(context, result);
		}
	}
}