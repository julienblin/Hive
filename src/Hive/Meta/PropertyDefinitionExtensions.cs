using Hive.Foundation.Extensions;

namespace Hive.Meta
{
	public static class PropertyDefinitionExtensions
	{
		public static IDataType GetValueTypeOrEntityDefinition(this IPropertyDefinition propertyDefinition, string name)
		{
			if (name.IsNullOrEmpty()) return null;

			return (IDataType)propertyDefinition.EntityDefinition.Model.Factories.ValueType.GetValueType(name)
				?? propertyDefinition.EntityDefinition.Model.EntitiesBySingleName.SafeGet(name);
		}
	}
}