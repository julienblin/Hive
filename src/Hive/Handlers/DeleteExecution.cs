using Hive.Meta;

namespace Hive.Handlers
{
	public class DeleteExecution
	{
		public DeleteExecution(IEntityDefinition entityDefinition, object id)
		{
			EntityDefinition = entityDefinition;
			Id = id;
		}

		public IEntityDefinition EntityDefinition { get; }

		public object Id { get; }
	}
}