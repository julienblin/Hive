using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Validation;

namespace Hive.Validation
{
	public interface IEntityValidationService
	{
		Task Validate(IEntity entity, CancellationToken ct);

		Task<ValidationResults> TryValidate(IEntity entity, CancellationToken ct);
	}
}