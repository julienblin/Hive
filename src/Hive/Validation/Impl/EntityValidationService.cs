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
		public async Task Validate(IEntity entity, CancellationToken ct)
		{
			var validationResults = await TryValidate(entity, ct);
			if (!validationResults.IsValid)
				throw new ValidationException(validationResults);
		}

		public Task<ValidationResults> TryValidate(IEntity entity, CancellationToken ct)
		{
			entity.NotNull(nameof(entity));

			var validationErrors = new List<ValidationError>();

			ValidateId(entity, validationErrors);

			return Task.FromResult(new ValidationResults(validationErrors));
		}

		private static void ValidateId(IEntity entity, List<ValidationError> validationErrors)
		{
			if (entity.Definition.EntityType == EntityType.None) return;

			if ((entity.Id == null) || (entity.Id is string && ((string) entity.Id).IsNullOrEmpty()))
				validationErrors.Add(new ValidationError(nameof(entity.Id), "A valid entity must have an id."));
		}
	}
}