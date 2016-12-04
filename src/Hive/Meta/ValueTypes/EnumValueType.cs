namespace Hive.Meta.ValueTypes
{
	public class EnumValueType : ValueType<string>
	{
		public EnumValueType()
			: base("enum")
		{
		}
	}
}