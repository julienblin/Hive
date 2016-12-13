using Microsoft.AspNetCore.Http;

namespace Hive.Web.Extensions
{
	public static class HttpRequestExtensions
	{
		public static string AbsoluteUri(this HttpRequest request, string relativeUri)
		{
			return string.Concat(
				request.Scheme,
				"://",
				request.Host.ToUriComponent(),
				request.PathBase.ToUriComponent(),
				relativeUri
			);
		}
	}
}