using System.Collections.Generic;
using System.IO;
using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Serialization.Impl
{
	public abstract class EntitySerializer : IEntitySerializer
	{
		protected EntitySerializer(IEnumerable<string> mediaTypes)
		{
			MediaTypes = mediaTypes.NotNull(nameof(mediaTypes));
		}

		public IEnumerable<string> MediaTypes { get; }

		public abstract IEntity Deserialize(IEntityDefinition entityDefinition, Stream stream);
	}
}