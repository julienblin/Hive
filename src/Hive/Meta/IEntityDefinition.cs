using System.Collections.Generic;

namespace Hive.Meta
{
	public interface IEntityDefinition : IDataType
	{
		string FullName { get; }

		IModel Model { get; }

		string SingleName { get; }

		string PluralName { get; }

		EntityType EntityType { get; }

		IReadOnlyDictionary<string, IPropertyDefinition> Properties { get; }
	}
}