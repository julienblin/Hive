namespace Hive.Meta
{
	public interface IValueTypeFactory
	{
		IValueType GetValueType(string name);
	}
}