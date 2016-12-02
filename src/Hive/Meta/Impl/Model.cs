using System.Collections.Generic;
using Hive.Foundation.Entities;

namespace Hive.Meta.Impl
{
	internal class Model : IModel
	{
		public string Name { get; set; }

		public SemVer Version { get; set; }

		public IReadOnlyDictionary<string, IEntityDefinition> EntitiesBySingleName { get; set; }

		public IReadOnlyDictionary<string, IEntityDefinition> EntitiesByPluralName { get; set; }
	}
}