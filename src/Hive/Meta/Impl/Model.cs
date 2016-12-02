using System.Collections.Generic;
using Hive.Foundation.Entities;
using Hive.Meta.Data;

namespace Hive.Meta.Impl
{
	internal class Model : IModel, IOriginalDataHolder<ModelData>
	{
		public string Name { get; set; }

		public SemVer Version { get; set; }

		public IReadOnlyDictionary<string, IEntityDefinition> EntitiesBySingleName { get; set; }

		public IReadOnlyDictionary<string, IEntityDefinition> EntitiesByPluralName { get; set; }

		public ModelData OriginalData { get; set; }

		internal void FinishLoading(IValueTypeFactory valueTypeFactory)
		{
			foreach (var entityDefinition in EntitiesBySingleName?.Values)
			{
				var ef = entityDefinition as EntityDefinition;
				if (ef != null)
				{
					ef.FinishLoading(valueTypeFactory, this);
				}
			}
		}

		public override string ToString() => Name;
	}
}