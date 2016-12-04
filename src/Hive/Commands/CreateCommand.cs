using Hive.Entities;
using Hive.Foundation.Extensions;

namespace Hive.Commands
{
	public class CreateCommand : Command<IEntity>
	{
		private readonly IEntity _entity;

		public CreateCommand(IEntity entity)
		{
			_entity = entity.NotNull(nameof(entity));
		}
	}
}