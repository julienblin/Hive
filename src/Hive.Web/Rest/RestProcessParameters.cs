using Hive.Meta;
using Hive.Web.Rest.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;

namespace Hive.Web.Rest
{
	public class RestProcessParameters
	{
		public RestProcessParameters(HttpContext context, RequestHeaders headers, string[] pathSegments, IModel model, IRestSerializer serializer)
		{
			Context = context;
			Headers = headers;
			PathSegments = pathSegments;
			Model = model;
			Serializer = serializer;
		}

		public HttpContext Context { get; set; }

		public RequestHeaders Headers { get; set; }

		public string[] PathSegments { get; set; }

		public IModel Model { get; set; }

		public IRestSerializer Serializer { get; set; }
	}
}