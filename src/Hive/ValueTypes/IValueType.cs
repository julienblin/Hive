using Hive.Meta;

namespace Hive.ValueTypes
{
	public interface IValueType : IDataType
	{
		void FinishLoading(IValueTypeFactory valueTypeFactory, IPropertyDefinition propertyDefinition);
	}
}
