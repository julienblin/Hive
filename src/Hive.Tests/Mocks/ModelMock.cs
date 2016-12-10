using System.Collections.Generic;
using System.Collections.Immutable;
using Hive.Entities;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.Meta.Impl;
using Hive.Validation;
using Hive.ValueTypes;

namespace Hive.Tests.Mocks
{
	public class ModelMock : IModel
	{
		public ModelMock(IEnumerable<IEntityDefinition> entityDefinitions = null, IModelFactories factories = null)
		{
			Name = "TestModel";
			Version = new SemVer(1, 0, 0);
			EntitiesBySingleName = entityDefinitions.Safe().ToImmutableDictionary(x => x.SingleName);
			EntitiesByPluralName = entityDefinitions.Safe().ToImmutableDictionary(x => x.PluralName);
			Factories = factories ?? new ModelFactoriesMock();
		}

		public string Name { get; }

		public SemVer Version { get; }

		public IImmutableDictionary<string, IEntityDefinition> EntitiesBySingleName { get; }

		public IImmutableDictionary<string, IEntityDefinition> EntitiesByPluralName { get; }

		public IModelFactories Factories { get; }

		public class ModelFactoriesMock : IModelFactories
		{
			public IValueTypeFactory ValueType { get; set; }

			public IValidatorFactory Validator { get; set; }

			public IEntityFactory Entity { get; set; }
		}
	}
}