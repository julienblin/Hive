using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Extensions;

namespace Hive.Handlers.Impl
{
	public class CreateHandler : ICreateHandler
	{
		private readonly IEntityRepository _entityRepository;

		public CreateHandler(IEntityRepository entityRepository)
		{
			_entityRepository = entityRepository.NotNull(nameof(entityRepository));
		}

		public Task<IEntity> Create(IEntity entity, CancellationToken ct)
		{
			entity.NotNull(nameof(entity));
			return _entityRepository.Create(entity, ct);
		}
	}
}