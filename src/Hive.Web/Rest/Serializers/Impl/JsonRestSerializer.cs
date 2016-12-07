using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Web.Rest.Serializers.Impl
{
	public class JsonRestSerializer : RestSerializer
	{
		public JsonRestSerializer()
			: base(new[] {"application/json"})
		{
		}

		protected override void SerializeEntity(IEntity entity, Stream stream)
		{
		}

		protected override void SerializeEntities(IEnumerable<IEntity> enumerable, Stream stream)
		{
		}

		protected override void SerializeMessage(object message, Stream stream)
		{
			HiveJsonSerializer.Instance.Serialize(message, stream);
		}

		public override Task<IEntity> Deserialize(IEntityDefinition entityDefinition, Stream stream, CancellationToken ct)
		{
			var propertyBag = HiveJsonSerializer.Instance.Deserialize<PropertyBag>(stream);
			return Task.FromResult<IEntity>(new Entity(entityDefinition, propertyBag));
		}
	}
}