using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta.Impl;

namespace Hive.Meta
{
	public static class PropertyDefinitionFactory
	{
		public static IPropertyDefinition Create(
			IEntityDefinition entityDefinition,
			IDataType dataType = null,
			PropertyBag propertyBag = null)
		{
			entityDefinition.NotNull(nameof(entityDefinition));

			return new PropertyDefinition
			{
				EntityDefinition = entityDefinition,
				PropertyType = dataType,
				PropertyBag = propertyBag,
				Name = propertyBag["name"] as string,
				Description = propertyBag["description"] as string,
				DefaultValue = propertyBag["default"]
			};
		}
	}
}