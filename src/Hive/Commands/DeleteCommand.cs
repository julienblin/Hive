using Hive.Context;
using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Commands
{
	public class DeleteCommand : Command
	{
		private readonly IEntityDefinition _entityDefinition;
		private readonly object _id;

		public DeleteCommand(IContext context, IEntity entity)
			: base(context)
		{
			_entityDefinition = entity.NotNull(nameof(entity)).Definition;
		}

		public DeleteCommand(IContext context, IEntityDefinition entityDefinition, object id)
			: base(context)
		{
			_entityDefinition = entityDefinition.NotNull(nameof(entityDefinition));
			// _id = entityDefinition.
		}
	}
}