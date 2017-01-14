using System;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;

namespace Hive.Data
{
	public interface IEntityRepository
	{
		Task<IEntity> Create(IEntity entity, CancellationToken ct);

		Task<IEntity> Update(IEntity entity, CancellationToken ct);

		Task<bool> Delete(Type modelType, object id, CancellationToken ct);
	}
}