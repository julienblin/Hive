using Hive.Entities;
using Hive.Foundation.Extensions;

namespace Hive.Commands
{
	public class UpdateCommand : Command<IEntity>
	{
		private readonly IEntity _entity;

		public UpdateCommand(IEntity entity)
		{
			_entity = entity.NotNull(nameof(entity));
		}
	}
}