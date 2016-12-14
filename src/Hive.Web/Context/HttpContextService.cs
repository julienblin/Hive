using System;
using System.Linq;
using System.Text.RegularExpressions;
using Hive.Context;
using Hive.Foundation.Extensions;
using Microsoft.AspNetCore.Http;

namespace Hive.Web.Context
{
	public class HttpContextService : IContextService
	{
		private const string HttpContextItemsKey = "HiveContext";
		private static readonly Regex UserAgentRegex = new Regex(@"(?<appname>[^/\s]+)\s*/\s*(?<appversion>[^\s,]+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);

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
				OperationId = operationIdHeader,
				ClientApplication = ParseUserAgent(httpContext.Request.Headers[WebConstants.UserAgentHeader].FirstOrDefault())
			};
			httpContext.Items[HttpContextItemsKey] = context;
			return context;
		}

		private ClientApplication ParseUserAgent(string userAgent)
		{
			if(userAgent.IsNullOrWhiteSpace())
				return ClientApplication.Unknown;

			var match = UserAgentRegex.Match(userAgent);
			if(match.Success)
				return new ClientApplication(match.Groups["appname"].Value, match.Groups["appversion"].Value);
			else
				return ClientApplication.Unknown;
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