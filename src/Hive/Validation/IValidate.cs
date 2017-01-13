using System.Threading;
using System.Threading.Tasks;

namespace Hive.Validation
{
	public interface IValidate<in T>
	{
		Task<ValidationResult> Validate(T @object, CancellationToken ct);
	}
}