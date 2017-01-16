using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;

namespace Hive.Handlers.Impl
{
	public class EntityRepositoryGetHandler<TEntity, TId> : IHandleGet<TEntity, TId>
		where TEntity : class, IEntity<TId>
	{
		public async Task<IHandlerResult> Get(TId id, CancellationToken ct)
		{
			return null;
		}
	}
}