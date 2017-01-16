using System;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;

namespace Hive.Data
{
	public interface IEntityRepository
	{
		Task<TEntity> Create<TEntity, TId>(TEntity entity, CancellationToken ct)
			where TEntity : class, IEntity<TId>;

		Task<TEntity> Update<TEntity, TId>(TEntity entity, CancellationToken ct)
			where TEntity : class, IEntity<TId>;

		Task<bool> Delete<TEntity, TId>(TId id, CancellationToken ct)
			where TEntity : class, IEntity<TId>;
	}
}