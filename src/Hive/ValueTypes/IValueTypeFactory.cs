namespace Hive.ValueTypes
{
	public interface IValueTypeFactory
	{
		IValueType GetValueType(string name);
	}
}