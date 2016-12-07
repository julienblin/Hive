using Hive.Meta;

namespace Hive.ValueTypes
{
	public class StringValueType : ValueType<string>
	{
		public StringValueType()
			: base("string")
		{
		}

		public override object ConvertFrom(IPropertyDefinition propertyDefinition, object value)
		{
			return value?.ToString();
		}
	}
}