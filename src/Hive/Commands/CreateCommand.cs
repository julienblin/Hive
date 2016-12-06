using Hive.Entities;
using Hive.Foundation.Extensions;

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