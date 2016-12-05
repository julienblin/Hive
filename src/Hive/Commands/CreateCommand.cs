using Hive.Entities;
using Hive.Foundation.Extensions;

namespace Hive.Commands
{
	public class CreateCommand : Command<IEntity>
	{
		public CreateCommand(IEntity entity)
		{
			Entity = entity.NotNull(nameof(entity));
		}

		public IEntity Entity { get; }
	}
}