namespace Hive.Meta
{
	public interface IPropertyDefinition
	{
		string Name { get; }

		IPropertyType PropertyType { get; }
	}
}