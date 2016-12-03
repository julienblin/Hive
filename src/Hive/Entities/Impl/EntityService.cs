using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Commands;
using Hive.Meta;
using Hive.Queries;

namespace Hive.Entities.Impl
{
	public class EntityService : IEntityService
	{
		public Task<IEntity> Get(IEntityDefinition entityDefinition, object id, CancellationToken ct)
		{
			throw new System.NotImplementedException();
		}

		public Task<IEnumerable<IDataType>> Execute(Query query, CancellationToken ct)
		{
			throw new System.NotImplementedException();
		}

		public Task Execute(Command command, CancellationToken ct)
		{
			throw new System.NotImplementedException();
		}
	}
}