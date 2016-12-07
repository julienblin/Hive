using Hive.Entities;

namespace Hive.Commands
{
	public class CreateCommand : EntityCommand
	{
		public CreateCommand(IEntity entity)
			: base(entity)
		{
		}
	}
}