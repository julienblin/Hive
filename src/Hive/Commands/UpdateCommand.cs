using Hive.Context;
using Hive.Entities;
using Hive.Foundation.Extensions;

namespace Hive.Commands
{
	public class UpdateCommand : Command
	{
		private readonly IEntity _entity;

		public UpdateCommand(IContext context, IEntity entity)
			: base(context)
		{
			_entity = entity.NotNull(nameof(entity));
		}
	}
}