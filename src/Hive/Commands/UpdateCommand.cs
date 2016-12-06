using Hive.Entities;
using Hive.Foundation.Extensions;

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