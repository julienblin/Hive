using System;
using System.Linq;
using Hive.Context;
using Hive.Foundation.Extensions;
using Microsoft.AspNetCore.Http;

namespace Hive.Web.Context
{
	public class HttpContextService : IContextService
	{
		private const string HttpContextItemsKey = "HiveContext";

		private readonly IHttpContextAccessor _contextAccessor;

		public HttpContextService(IHttpContextAccessor contextAccessor)
		{
			_contextAccessor = contextAccessor.NotNull(nameof(contextAccessor));
		}

		public IContext StartContext()
		{
			var httpContext = _contextAccessor.HttpContext;
			var operationIdHeader = httpContext.Request.Headers[WebConstants.OperationIdHeader].FirstOrDefault() ?? Guid.NewGuid().ToString();
			httpContext.TraceIdentifier = operationIdHeader;
			var context = new HttpContextContext
			{
				OperationId = operationIdHeader
			};
			httpContext.Items[HttpContextItemsKey] = context;
			return context;
		}

		public IContext GetContext()
		{
			var httpContext = _contextAccessor.HttpContext;
			return httpContext.Items[HttpContextItemsKey] as IContext;
		}

		public void StopContext()
		{
			var httpContext = _contextAccessor.HttpContext;
			httpContext.Items[HttpContextItemsKey] = null;
		}
	}
}