using System.Collections.Generic;
using Hive.Foundation.Entities;
using Hive.ValueTypes;

namespace Hive.Meta.Impl
{
	internal class Model : IModel
	{
		public PropertyBag PropertyBag { get; set; }
		public string Name { get; set; }

		public SemVer Version { get; set; }

		public IReadOnlyDictionary<string, IEntityDefinition> EntitiesBySingleName { get; set; }

		public IReadOnlyDictionary<string, IEntityDefinition> EntitiesByPluralName { get; set; }

		internal void FinishLoading(IValueTypeFactory valueTypeFactory)
		{
			foreach (var entityDefinition in EntitiesBySingleName?.Values)
			{
				var ef = entityDefinition as EntityDefinition;
				if (ef != null)
					ef.FinishLoading(valueTypeFactory);
			}
		}

		public override string ToString() => Name;
	}
}