using System;
using System.Collections.Generic;
using System.Linq;
using Hive.Foundation.Entities;
using Hive.Meta;
using Hive.Foundation.Extensions;

namespace Hive.Tests.Mocks
{
	public class ModelMock : IModel
	{
		public ModelMock(IEnumerable<IEntityDefinition> entityDefinitions = null)
		{
			Name = "TestModel";
			Version = new SemVer(1,0,0);
			EntitiesBySingleName = entityDefinitions.Safe().ToDictionary(x => x.SingleName);
			EntitiesByPluralName = entityDefinitions.Safe().ToDictionary(x => x.PluralName);
		}

		public string Name { get; }
		public SemVer Version { get; }
		public IReadOnlyDictionary<string, IEntityDefinition> EntitiesBySingleName { get; }
		public IReadOnlyDictionary<string, IEntityDefinition> EntitiesByPluralName { get; }
	}
}