using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Commands;
using Hive.Meta;
using Hive.Queries;

namespace Hive.Entities
{
	public interface IEntityRepository
	{
		Task<T> Execute<T>(Query<T> query, CancellationToken ct);

		Task<IEntity> Create(IEntity entity, CancellationToken ct);
			
		Task<IEntity> Update(IEntity entity, CancellationToken ct);
	}
}