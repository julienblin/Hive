using System.Collections.Generic;
using System.Collections.Immutable;
using Hive.Exceptions;
using Hive.Foundation.Extensions;

namespace Hive.Web.Rest.Serializers.Impl
{
	public class RestSerializerFactory : IRestSerializerFactory
	{
		private readonly IReadOnlyDictionary<string, IRestSerializer> _serializersByMediaTypes;

		public RestSerializerFactory(IEnumerable<IRestSerializer> serializers = null)
		{
			var realSerializers = serializers.IsNullOrEmpty()
				? new[] {new JsonRestSerializer()}
				: serializers;
			_serializersByMediaTypes = LoadSerializers(realSerializers);
		}

		public IRestSerializer GetByMediaType(string mediaType)
		{
			var result = _serializersByMediaTypes.SafeGet(mediaType);
			if (result == null)
				throw new SerializationException($"Unable to find a suitable serializer for media type {mediaType}.");
			return result;
		}

		public IRestSerializer GetByMediaType(IEnumerable<string> mediaTypes)
		{
			if(mediaTypes.IsNullOrEmpty())
				throw new SerializationException($"Unable to find a suitable serializer because no media type was provided.");

			foreach (var mediaType in mediaTypes)
			{
				var result = _serializersByMediaTypes.SafeGet(mediaType);
				if (result != null) return result;
			}
			throw new SerializationException($"Unable to find a suitable serializer for media types {string.Join(",", mediaTypes)}.");
		}

		private static IReadOnlyDictionary<string, IRestSerializer> LoadSerializers(IEnumerable<IRestSerializer> serializers)
		{
			var serializersByMediaTypes = new Dictionary<string, IRestSerializer>();

			foreach (var serializer in serializers)
			{
				foreach (var mediaType in serializer.MediaTypes)
				{
					serializersByMediaTypes[mediaType] = serializer;
				}
			}

			return serializersByMediaTypes.ToImmutableDictionary();
		}
	}
}