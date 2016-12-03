using System.Collections.Generic;
using System.IO;
using Hive.Entities;
using Hive.Meta;

namespace Hive.Serialization
{
	public interface IEntitySerializer
	{
		IEnumerable<string> MediaTypes { get; }

		IEntity Deserialize(IEntityDefinition entityDefinition, Stream stream);
	}
}