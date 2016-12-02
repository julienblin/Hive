using System.Collections.Generic;
using Hive.Foundation.Entities;
using Hive.Meta.Data;

namespace Hive.Meta
{
	public interface IModel
	{
		string Name { get; }

		SemVer Version { get; }

		IReadOnlyDictionary<string, IEntityDefinition> EntitiesBySingleName { get; }

		IReadOnlyDictionary<string, IEntityDefinition> EntitiesByPluralName { get; }
	}
}