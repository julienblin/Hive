using Hive.Meta.Data;

namespace Hive.Meta
{
	public interface IPropertyDefinition
	{
		string Name { get; }

		IDataType PropertyType { get; }

		T GetProperty<T>(string propertyName);

		void SetProperty(string propertyName, object value);
	}
}