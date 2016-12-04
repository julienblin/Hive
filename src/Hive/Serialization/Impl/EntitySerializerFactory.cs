using System.Collections.Generic;
using System.Collections.Immutable;
using Hive.Exceptions;
using Hive.Foundation.Extensions;

namespace Hive.Serialization.Impl
{
	public class EntitySerializerFactory : IEntitySerializerFactory
	{
		private readonly IReadOnlyDictionary<string, IEntitySerializer> _serializersByMediaTypes;

		public EntitySerializerFactory()
			: this(new []{ new JsonEntitySerializer() })
		{
		}

		public EntitySerializerFactory(IEnumerable<IEntitySerializer> serializers)
		{
			_serializersByMediaTypes = LoadSerializers(serializers.NotNull(nameof(serializers)));
		}

		public IEntitySerializer GetByMediaType(string mediaType)
		{
			var result = _serializersByMediaTypes.SafeGet(mediaType);
			if (result == null)
				throw new SerializationException($"Unable to find a suitable serializer for media type {mediaType}.");
			return result;
		}

		private static IReadOnlyDictionary<string, IEntitySerializer> LoadSerializers(IEnumerable<IEntitySerializer> serializers)
		{
			var serializersByMediaTypes = new Dictionary<string, IEntitySerializer>();

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