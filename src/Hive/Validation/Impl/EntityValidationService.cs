using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Exceptions;
using Hive.Foundation.Extensions;
using Hive.Foundation.Validation;
using Hive.Meta;

namespace Hive.Validation.Impl
{
	public class EntityValidationService : IEntityValidationService
	{
		private readonly IValidatorFactory _validatorFactory;

		public EntityValidationService(IValidatorFactory validatorFactory)
		{
			_validatorFactory = validatorFactory.NotNull(nameof(validatorFactory));
		}

		public async Task Validate(IEntity entity, CancellationToken ct)
		{
			var validationResults = await TryValidate(entity, ct);
			if (!validationResults.IsValid)
				throw new ValidationException(validationResults);
		}

		public async Task<ValidationResults> TryValidate(IEntity entity, CancellationToken ct)
		{
			entity.NotNull(nameof(entity));

			var validationErrors = new List<ValidationError>();

			ValidateId(entity, validationErrors);

			foreach (var propertyDefinition in entity.Definition.Properties.Values.Safe())
			{
				foreach (var validatorDefinition in propertyDefinition.ValidatorDefinitions.Safe())
				{
					validationErrors.AddRange(await validatorDefinition.Validator.Validate(validatorDefinition, entity[propertyDefinition.Name], ct));
				}
			}

			return new ValidationResults(validationErrors);
		}

		private static void ValidateId(IEntity entity, List<ValidationError> validationErrors)
		{
			if (entity.Definition.EntityType == EntityType.None) return;

			if ((entity.Id == null) || (entity.Id is string && ((string) entity.Id).IsNullOrEmpty()))
				validationErrors.Add(new ValidationError(nameof(entity.Id), "A valid entity must have an id."));
		}
	}
}