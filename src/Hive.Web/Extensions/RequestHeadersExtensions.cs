using Hive.Foundation.Extensions;
using Microsoft.AspNetCore.Http.Headers;
using System.Linq;

namespace Hive.Web.Extensions
{
	public static class RequestHeadersExtensions
	{
		public static string IfNoneMatch(this RequestHeaders requestHeaders)
		{
			return requestHeaders.NotNull(nameof(requestHeaders)).IfNoneMatch.Safe().FirstOrDefault()?.Tag;
		}
	}
}