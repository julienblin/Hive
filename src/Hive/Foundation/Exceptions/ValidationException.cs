using Hive.Foundation.Extensions;
using Hive.Foundation.Validation;

namespace Hive.Foundation.Exceptions
{
	public class ValidationException : HiveException
	{
		public ValidationException(ValidationResults results)
		{
			Results = results.NotNull(nameof(results));
		}

		public ValidationException(ValidationError error)
			: this(new ValidationResults(new[] { error }))
		{
		}

		public ValidationResults Results { get; }
	}
}