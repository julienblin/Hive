using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Queries
{
	public abstract class EntityDefinitionQuery<T> : Query<T>
	{
		protected EntityDefinitionQuery(IEntityDefinition entityDefinition)
		{
			EntityDefinition = entityDefinition.NotNull(nameof(entityDefinition));
		}

		public IEntityDefinition EntityDefinition { get; set; }
	}
}