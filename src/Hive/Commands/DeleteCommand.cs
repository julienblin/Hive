using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Commands
{
	public class DeleteCommand : Command<IEntity>
	{
		private readonly IEntityDefinition _entityDefinition;
		private readonly object _id;

		public DeleteCommand(IEntity entity)
		{
			_entityDefinition = entity.NotNull(nameof(entity)).Definition;
			_id = entity.Id;
		}

		public DeleteCommand(IEntityDefinition entityDefinition, object id)
		{
			_entityDefinition = entityDefinition.NotNull(nameof(entityDefinition));
			_id = id.NotNull(nameof(id));
		}
	}
}