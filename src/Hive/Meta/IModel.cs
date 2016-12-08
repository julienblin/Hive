using System.Collections.Immutable;
using Hive.Foundation.Entities;

namespace Hive.Meta
{
	public interface IModel
	{
		string Name { get; }

		SemVer Version { get; }

		IImmutableDictionary<string, IEntityDefinition> EntitiesBySingleName { get; }

		IImmutableDictionary<string, IEntityDefinition> EntitiesByPluralName { get; }
	}
}