namespace Hive.Meta.ValueTypes
{
	public class StringValueType : ValueType<string>
	{
		public StringValueType()
			: base("string")
		{
		}

		public override object ConvertValue(IPropertyDefinition propertyDefinition, object value)
		{
			return value?.ToString();
		}
	}
}