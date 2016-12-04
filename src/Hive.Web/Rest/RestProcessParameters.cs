using Hive.Meta;
using Hive.Web.Rest.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;

namespace Hive.Web.Rest
{
	public class RestProcessParameters
	{
		public RestProcessParameters(HttpContext context, RequestHeaders headers, string[] pathSegments, IModel model, IRestSerializer requestSerializer, IRestSerializer responseSerializer)
		{
			Context = context;
			Headers = headers;
			PathSegments = pathSegments;
			Model = model;
			RequestSerializer = requestSerializer;
			ResponseSerializer = responseSerializer;
		}

		public HttpContext Context { get; }

		public RequestHeaders Headers { get; }

		public string[] PathSegments { get; }

		public IModel Model { get; }

		public IRestSerializer RequestSerializer { get; set; }

		public IRestSerializer ResponseSerializer { get; set; }
	}
}