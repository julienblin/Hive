using System.Collections.Immutable;

namespace Hive.Meta
{
	public interface IEntityDefinition : IDataType
	{
		string FullName { get; }

		IModel Model { get; }

		string SingleName { get; }

		string PluralName { get; }

		EntityType EntityType { get; }

		IImmutableDictionary<string, IPropertyDefinition> Properties { get; }
	}
}