using System.IO;
using Hive.Entities;
using Hive.Meta;

namespace Hive.Serialization.Impl
{
	public class JsonEntitySerializer : EntitySerializer
	{
		public JsonEntitySerializer()
			: base (new []{ "application/json" })
		{
		}

		public override IEntity Deserialize(IEntityDefinition entityDefinition, Stream stream)
		{
			throw new System.NotImplementedException();
		}
	}
}