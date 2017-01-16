using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;

namespace Hive.Handlers.Impl
{
	public class EntityRepositoryDeleteHandler<TEntity, TId> : IHandleDelete<TEntity, TId>
		where TEntity : class, IEntity<TId>
	{
		public Task<IHandlerResult> Delete(TId id, CancellationToken ct)
		{
			throw new System.NotImplementedException();
		}
	}
}