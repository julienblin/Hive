using System.Collections.Immutable;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.ValueTypes;

namespace Hive.Meta.Impl
{
	internal class Model : IModel
	{
		public PropertyBag PropertyBag { get; set; }

		public string Name { get; set; }

		public SemVer Version { get; set; }

		public IImmutableDictionary<string, IEntityDefinition> EntitiesBySingleName { get; set; }

		public IImmutableDictionary<string, IEntityDefinition> EntitiesByPluralName { get; set; }

		public IValueTypeFactory ValueTypeFactory { get; set; }

		internal void ModelLoaded() => EntitiesBySingleName?.Values.SafeForEach(x => ((EntityDefinition)x).ModelLoaded());

		public override string ToString() => Name;
	}
}