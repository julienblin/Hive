using System.Collections.Generic;
using System.Linq;
using Hive.Foundation.Extensions;

namespace Hive.Foundation.Validation
{
	public class ValidationResults
	{
		public ValidationResults(IEnumerable<ValidationError> errors)
		{
			Errors = errors;
		}

		public bool IsValid => !Errors.Safe().Any();

		public IEnumerable<ValidationError> Errors { get; }

		public override string ToString() => $"Valid: {IsValid} ({string.Join(", ", Errors.Safe())})";
	}
}