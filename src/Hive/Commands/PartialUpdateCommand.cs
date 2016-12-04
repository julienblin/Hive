using Hive.Entities;
using Hive.Foundation.Extensions;

namespace Hive.Commands
{
	public class PartialUpdateCommand : Command<IEntity>
	{
		private readonly IEntity _entity;

		public PartialUpdateCommand(IEntity entity)
		{
			_entity = entity.NotNull(nameof(entity));
		}
	}
}