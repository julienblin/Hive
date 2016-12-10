using System.Collections.Generic;
using System.Collections.Immutable;
using Hive.Foundation.Extensions;
using Hive.ValueTypes;

namespace Hive.Validation.Impl
{
	public class ValidatorFactory : IValidatorFactory
	{
		private readonly IImmutableDictionary<string, IValidator> _validators;

		public ValidatorFactory(IEnumerable<IValidator> validators = null)
		{
			_validators = validators.Safe().ToImmutableDictionary(x => x.Name);
		}

		public IValidator GetValidator(string name)
		{
			return _validators.SafeGet(name);
		}
	}
}