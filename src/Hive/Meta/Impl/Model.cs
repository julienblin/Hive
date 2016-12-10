using System.Collections.Immutable;
using Hive.Entities;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Validation;
using Hive.ValueTypes;

namespace Hive.Meta.Impl
{
	internal class Model : IModel
	{
		public Model(string name, SemVer version, IModelFactories factories)
		{
			Name = name.NotNullOrEmpty(nameof(name));
			Version = version.NotNull(nameof(version));
			Factories = factories.NotNull(nameof(factories));
		}

		public PropertyBag PropertyBag { get; set; }

		public string Name { get; }

		public SemVer Version { get; }

		public IImmutableDictionary<string, IEntityDefinition> EntitiesBySingleName { get; set; }

		public IImmutableDictionary<string, IEntityDefinition> EntitiesByPluralName { get; set; }

		public IModelFactories Factories { get; }

		internal void ModelLoaded() => EntitiesBySingleName?.Values.SafeForEach(x => ((EntityDefinition)x).ModelLoaded());

		public override string ToString() => Name;
	}
}