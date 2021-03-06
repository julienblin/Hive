﻿using Hive.Foundation.Extensions;
using Hive.Foundation.Validation;

namespace Hive.Exceptions
{
	public class ValidationException : HiveException
	{
		public ValidationException(ValidationResults results)
		{
			Results = results.NotNull(nameof(results));
		}

		public ValidationException(ValidationError error)
			: this(new ValidationResults(new[] {error}))
		{
		}

		public ValidationResults Results { get; }

		public override string ToString() => $"{base.ToString()}: {Results}";
	}
}