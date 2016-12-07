using System.Collections.Generic;
using Hive.Entities;
using Hive.Meta;

namespace Hive.Queries
{
	public class ListQuery : EntityDefinitionQuery<IEnumerable<IEntity>>
	{
		public ListQuery(IEntityDefinition entityDefinition)
			: base(entityDefinition)
		{
		}

		public int? Limit { get; set; }
	}
}