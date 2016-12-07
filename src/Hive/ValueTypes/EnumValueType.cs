namespace Hive.ValueTypes
{
	public class EnumValueType : ValueType<string>
	{
		public EnumValueType()
			: base("enum")
		{
		}
	}
}