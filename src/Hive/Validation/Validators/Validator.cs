using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Foundation.Extensions;
using Hive.Foundation.Validation;
using Hive.Meta;

namespace Hive.Validation.Validators
{
	public abstract class Validator : IValidator
	{
		protected Validator(string name)
		{
			Name = name.NotNullOrEmpty(nameof(name));
		}

		public string Name { get; }

		public virtual void ModelLoaded(IPropertyValidatorDefinition validatorDefinition)
		{
		}

		public virtual Task<IEnumerable<ValidationError>> Validate(
			IPropertyValidatorDefinition validatorDefinition,
			object value,
			CancellationToken ct)
		{
			return Task.FromResult(ValidateSync(validatorDefinition, value));
		}

		protected abstract IEnumerable<ValidationError> ValidateSync(IPropertyValidatorDefinition validatorDefinition, object value);
	}
}