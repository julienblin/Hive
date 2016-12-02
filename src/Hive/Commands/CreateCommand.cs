using Hive.Context;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Commands
{
	public class CreateCommand : Command
	{
		private readonly IEntityDefinition _entityDefinition;
		private readonly string _payload;

		public CreateCommand(IContext context, IEntityDefinition entityDefinition, string payload = null)
			: base(context)
		{
			_entityDefinition = entityDefinition.NotNull(nameof(entityDefinition));
			_payload = payload;
		}
	}
}