using System;
using System.Collections.Generic;
using Hive.Foundation.Entities;
using Hive.Validation;

namespace Hive.Meta.Impl
{
	public class PropertyValidatorDefinition : IPropertyValidatorDefinition
	{
		private readonly Lazy<IDictionary<string, object>> _additionalProperties
			= new Lazy<IDictionary<string, object>>(() => new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));

		public IPropertyDefinition PropertyDefinition { get; set; }

		public IValidator Validator { get; set; }

		public string Message { get; set; }

		public IDictionary<string, object> AdditionalProperties => _additionalProperties.Value;

		public PropertyBag PropertyBag { get; set; }

		public override string ToString() => $"{PropertyDefinition}.{Validator.Name}";
	}
}