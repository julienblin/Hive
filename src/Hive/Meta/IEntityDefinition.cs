using System.Collections.Generic;
using Hive.Meta.Data;

namespace Hive.Meta
{
	public interface IEntityDefinition : IDataType
	{
		string SingleName { get;  }

		string PluralName { get; }

		EntityType EntityType { get; }

		IReadOnlyDictionary<string, IPropertyDefinition> Properties { get; }
	}
}