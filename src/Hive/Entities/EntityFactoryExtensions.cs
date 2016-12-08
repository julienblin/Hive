using System.Threading;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Entities
{
	public static class EntityFactoryExtensions
	{
		public static IEntity Create(this IEntityFactory entityFactory, IEntityDefinition definition)
		{
			entityFactory.NotNull(nameof(entityFactory));
			definition.NotNull(nameof(definition));

			return entityFactory.Create(definition, CancellationToken.None).GetAwaiter().GetResult();
		}

		public static IEntity Create(this IEntityFactory entityFactory, IEntityDefinition definition, PropertyBag initialValues)
		{
			entityFactory.NotNull(nameof(entityFactory));
			definition.NotNull(nameof(definition));

			return entityFactory.Create(definition, initialValues, CancellationToken.None).GetAwaiter().GetResult();
		}

		public static IEntity Hydrate(this IEntityFactory entityFactory, IEntityDefinition definition, PropertyBag values)
		{
			entityFactory.NotNull(nameof(entityFactory));
			definition.NotNull(nameof(definition));

			return entityFactory.Hydrate(definition, values, CancellationToken.None).GetAwaiter().GetResult();
		}
	}
}