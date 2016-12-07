using Hive.Entities;

namespace Hive.Commands
{
	public class UpdateCommand : EntityCommand
	{
		public UpdateCommand(IEntity entity)
			: base(entity)
		{
		}
	}
}