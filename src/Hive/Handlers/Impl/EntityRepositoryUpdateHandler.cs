using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;

namespace Hive.Handlers.Impl
{
	public class EntityRepositoryUpdateHandler<TEntity, TId> : IHandleUpdate<TEntity>
		where TEntity : class, IEntity<TId>
	{
		public Task<IHandlerResult> Update(TEntity resource, CancellationToken ct)
		{
			throw new System.NotImplementedException();
		}
	}
}