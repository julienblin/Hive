using System.Collections.Generic;
using System.Collections.Immutable;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.Validation;
using Hive.ValueTypes;

namespace Hive.Tests.Mocks
{
	public class ModelMock : IModel
	{
		public ModelMock(IEnumerable<IEntityDefinition> entityDefinitions = null)
		{
			Name = "TestModel";
			Version = new SemVer(1, 0, 0);
			EntitiesBySingleName = entityDefinitions.Safe().ToImmutableDictionary(x => x.SingleName);
			EntitiesByPluralName = entityDefinitions.Safe().ToImmutableDictionary(x => x.PluralName);
		}

		public string Name { get; }

		public SemVer Version { get; }

		public IImmutableDictionary<string, IEntityDefinition> EntitiesBySingleName { get; }

		public IImmutableDictionary<string, IEntityDefinition> EntitiesByPluralName { get; }

		public IValueTypeFactory ValueTypeFactory { get; set; }

		public IValidatorFactory ValidatorFactory { get; set; }
	}
}