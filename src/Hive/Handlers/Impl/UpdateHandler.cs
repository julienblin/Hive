using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Extensions;

namespace Hive.Handlers.Impl
{
	public class UpdateHandler : IUpdateHandler
	{
		private readonly IEntityRepository _entityRepository;

		public UpdateHandler(IEntityRepository entityRepository)
		{
			_entityRepository = entityRepository.NotNull(nameof(entityRepository));
		}

		public Task<IEntity> Update(IEntity entity, CancellationToken ct)
		{
			entity.NotNull(nameof(entity));
			return _entityRepository.Update(entity, ct);
		}
	}
}