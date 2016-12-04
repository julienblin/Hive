using Hive.Entities;
using Hive.Meta;

namespace Hive.Queries
{
	public sealed class IdQuery : EntityDefinitionQuery<IEntity>
	{
		public IdQuery(IEntityDefinition entityDefinition, object id)
			: base(entityDefinition)
		{
			Id = id;
		}

		public object Id { get; }
	}
}