using System.Collections.Generic;
using Hive.Validation;

namespace Hive.Meta
{
	public interface IPropertyValidatorDefinition
	{
		IPropertyDefinition PropertyDefinition { get; }

		IValidator Validator { get; }

		IDictionary<string, object> AdditionalProperties { get; }
	}
}