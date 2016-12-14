using System;
using System.Threading;
using System.Threading.Tasks;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.Queries;
using Hive.Validation;

namespace Hive.Entities.Impl
{
	public class EntityService : IEntityService
	{
		private readonly IEntityRepository _entityRepository;
		private readonly IEntityValidationService _validationService;

		public EntityService(IEntityValidationService validationService, IEntityRepository entityRepository)
		{
			_validationService = validationService.NotNull(nameof(validationService));
			_entityRepository = entityRepository.NotNull(nameof(entityRepository));
		}

		public IQuery CreateQuery(IEntityDefinition entityDefinition)
		{
			entityDefinition.NotNull(nameof(entityDefinition));

			//TODO: Might allow security/audit interception capability later...
			return _entityRepository.CreateQuery(entityDefinition);
		}

		public async Task<IEntity> Create(IEntity entity, CancellationToken ct)
		{
			entity.NotNull(nameof(entity));
			await _validationService.Validate(entity, ct);
			return await _entityRepository.Create(entity, ct);
		}

		public async Task<IEntity> Update(IEntity entity, CancellationToken ct)
		{
			entity.NotNull(nameof(entity));
			await _validationService.Validate(entity, ct);
			return await _entityRepository.Update(entity, ct);
		}

		public Task<bool> Delete(IEntityDefinition entityDefinition, object id, CancellationToken ct)
		{
			entityDefinition.NotNull(nameof(entityDefinition));
			id.NotNull(nameof(id));

			return _entityRepository.Delete(entityDefinition, id, ct);
		}
	}
}