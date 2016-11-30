using System.Collections.Generic;

namespace Hive.Meta
{
	public interface IEntityDefinition
	{
		string SingleName { get;  }

		string PluralName { get; }

		EntityType EntityType { get; }

		IReadOnlyDictionary<string, IPropertyDefinition> Properties { get; set; }
	}
}