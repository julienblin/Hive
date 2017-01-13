using System.Collections.Generic;
using Hive.Foundation.Extensions;
using System.Linq;

namespace Hive.Validation
{
	public class ValidationResult
	{
		public ValidationResult(IEnumerable<ValidationError> errors)
		{
			Errors = errors;
		}

		public bool IsValid => !Errors.Safe().Any();

		public IEnumerable<ValidationError> Errors { get; }

		public override string ToString() => $"Valid: {IsValid} ({string.Join(", ", Errors.Safe())})";
	}
}