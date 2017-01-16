using System.Threading;
using System.Threading.Tasks;
using Hive.Data;
using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Handlers.Results;

namespace Hive.Handlers.Impl
{
	public class EntityRepositoryCreateHandler<TEntity, TId> : IHandleCreate<TEntity>
		where TEntity : class, IEntity<TId>
	{
		private readonly IEntityRepository _entityRepository;

		public EntityRepositoryCreateHandler(IEntityRepository entityRepository)
		{
			_entityRepository = entityRepository.NotNull(nameof(entityRepository));
		}

		public async Task<IHandlerResult> Create(TEntity resource, CancellationToken ct)
		{
			var createdEntity = await _entityRepository.Create<TEntity, TId>(resource, ct);
			return new CreatedResult(createdEntity);
		}
	}
}