using System.Collections.Generic;
using Hive.Foundation.Extensions;

namespace Hive.Meta.Impl
{
	internal class EntityDefinition : IEntityDefinition
	{
		public string Name => SingleName;

		public string SingleName { get; set; }

		public string PluralName { get; set; }

		public EntityType EntityType { get; set; }

		public IReadOnlyDictionary<string, IPropertyDefinition> Properties { get; set; }

		internal void ResolveReferences(IValueTypeFactory valueTypeFactory, Model model)
		{
			foreach (PropertyDefinition propertyDefinition in Properties.Values)
			{
				if (propertyDefinition.PropertyType == null)
				{
					propertyDefinition.PropertyType = model.EntitiesBySingleName.SafeGet(propertyDefinition.PropertyDefinitionData.Type);
				}

				propertyDefinition.Properties =
					((IValueType) propertyDefinition.PropertyType).LoadAdditionalProperties(valueTypeFactory, model, propertyDefinition.PropertyDefinitionData);
			}
		}
	}
}