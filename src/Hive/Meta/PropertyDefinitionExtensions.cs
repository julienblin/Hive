using Hive.Foundation.Extensions;

namespace Hive.Meta
{
	public static class PropertyDefinitionExtensions
	{
		public static IDataType GetValueTypeOrEntityDefinition(this IPropertyDefinition propertyDefinition, string name)
		{
			if (name.IsNullOrEmpty()) return null;

			return (IDataType)propertyDefinition.EntityDefinition.Model.ValueTypeFactory.GetValueType(name)
				?? propertyDefinition.EntityDefinition.Model.EntitiesBySingleName.SafeGet(name);
		}
	}
}