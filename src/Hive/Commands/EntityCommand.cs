using Hive.Entities;
using Hive.Foundation.Extensions;

namespace Hive.Commands
{
	public abstract class EntityCommand : Command<IEntity>
	{
		protected EntityCommand(IEntity entity)
		{
			Entity = entity.NotNull(nameof(entity));
		}

		public IEntity Entity { get; }
	}
}