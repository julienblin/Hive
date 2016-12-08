using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Commands
{
	public class DeleteCommand : Command<bool>
	{
		public DeleteCommand(IEntity entity)
		{
			EntityDefinition = entity.NotNull(nameof(entity)).Definition;
			EntityId = entity.Id;
		}

		public DeleteCommand(IEntityDefinition entityDefinition, object id)
		{
			EntityDefinition = entityDefinition.NotNull(nameof(entityDefinition));
			EntityId = id.NotNull(nameof(id));
		}

		public IEntityDefinition EntityDefinition { get; }

		public object EntityId { get; }
	}
}