using Hive.ValueTypes;

namespace Hive.Meta.Impl
{
	internal class DefaultIdPropertyDefinition : PropertyDefinition
	{
		public DefaultIdPropertyDefinition(IEntityDefinition entityDefinition, IDataType guidDataType)
		{
			Name = MetaConstants.IdProperty;
			EntityDefinition = entityDefinition;
			PropertyType = guidDataType;
			DefaultValue = GuidValueType.NewGuidDefaultValue;
		}
	}
}