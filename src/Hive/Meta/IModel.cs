using System.Collections.Generic;
using Hive.Foundation.Entities;

namespace Hive.Meta
{
	public interface IModel
	{
		string Name { get; }

		SemVer Version { get; }

		IReadOnlyDictionary<string, IEntityDefinition> Entities { get; }
	}
}