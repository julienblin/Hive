using System.Collections.Generic;
using Hive.Foundation.Entities;
using Hive.Validation;

namespace Hive.Meta
{
	public interface IPropertyValidatorDefinition
	{
		IPropertyDefinition PropertyDefinition { get; }

		IValidator Validator { get; }

		string Message { get; }

		IDictionary<string, object> AdditionalProperties { get; }

		PropertyBag PropertyBag { get; }
	}
}