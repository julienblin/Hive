using Hive.Meta;

namespace Hive.ValueTypes
{
	public class StringValueType : ValueType<string>
	{
		public StringValueType()
			: base("string")
		{
		}

		public override object ConvertFromPropertyBagValue(IPropertyDefinition propertyDefinition, object value)
		{
			return value?.ToString();
		}
	}
}