using System.Collections.Generic;
using Hive.Foundation.Validation;
using Hive.Meta;

namespace Hive.Validation.Validators
{
	public class RequiredValidator : Validator
	{
		public RequiredValidator()
			: base("required")
		{
		}

		protected override IEnumerable<ValidationError> ValidateSync(IPropertyValidatorDefinition validatorDefinition, object value)
		{
			if (value == null)
			{
				yield return CreateError(validatorDefinition, $"{validatorDefinition.PropertyDefinition.Name} is required.");
			}

			if (value is string)
			{
				if (value.Equals(string.Empty))
				{
					yield return CreateError(validatorDefinition, $"{validatorDefinition.PropertyDefinition.Name} is required.");
				}
			}
		}
	}
}