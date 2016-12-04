using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Web.Rest.Serializers.Impl
{
	public abstract class RestSerializer : IRestSerializer
	{
		protected RestSerializer(IEnumerable<string> mediaTypes)
		{
			MediaTypes = mediaTypes.NotNull(nameof(mediaTypes));
		}

		public IEnumerable<string> MediaTypes { get; }

		public virtual void Serialize(object @object, Stream stream)
		{
			if (@object is IEntity)
			{
				SerializeEntity((IEntity) @object, stream);
				return;
			}

			if (@object is IEnumerable<IEntity>)
			{
				SerializeEntities((IEnumerable<IEntity>)@object, stream);
				return;
			}

			SerializeMessage(@object, stream);
		}

		public abstract Task<IEntity> Deserialize(IEntityDefinition entityDefinition, Stream stream, CancellationToken ct);

		protected abstract void SerializeEntity(IEntity entity, Stream stream);

		protected abstract void SerializeEntities(IEnumerable<IEntity> enumerable, Stream stream);

		protected abstract void SerializeMessage(object message, Stream stream);
	}
}