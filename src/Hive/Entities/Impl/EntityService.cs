using System;
using System.Threading;
using System.Threading.Tasks;
using Hive.Commands;
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

		public Task<T> Execute<T>(Command<T> command, CancellationToken ct)
		{
			if (command is CreateCommand)
				return Execute<T>(command as CreateCommand, ct);

			if (command is UpdateCommand)
				return Execute<T>(command as UpdateCommand, ct);

			if (command is DeleteCommand)
				return Execute<T>(command as DeleteCommand, ct);

			throw new NotImplementedException();
		}

		private async Task<T> Execute<T>(CreateCommand command, CancellationToken ct)
		{
			await _validationService.Validate(command.Entity, ct);
			return (T) await _entityRepository.Create(command.Entity, ct);
		}

		private async Task<T> Execute<T>(UpdateCommand command, CancellationToken ct)
		{
			await _validationService.Validate(command.Entity, ct);
			return (T) await _entityRepository.Update(command.Entity, ct);
		}

		private async Task<T> Execute<T>(DeleteCommand command, CancellationToken ct)
		{
			return (T) (object) await _entityRepository.Delete(command.EntityDefinition, command.EntityId, ct);
		}
	}
}