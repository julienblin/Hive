using System;
using System.Collections.Generic;
using Hive.Exceptions;
using Hive.Meta;
using System.Linq;

namespace Hive.ValueTypes
{
	public class EnumValueType : ValueType<string>
	{
		private const string PropertyValues = "values";

		public EnumValueType()
			: base("enum")
		{
		}

		public override object ConvertFromPropertyBagValue(IPropertyDefinition propertyDefinition, object value)
		{
			var strValue = value as string;
			if (strValue == null) return null;

			var enumValues = (IDictionary<string, string>)propertyDefinition.AdditionalProperties[PropertyValues];

			if (!enumValues.ContainsKey(strValue))
				throw new ValueTypeException(this, $"{strValue} is not a valid enum value for {propertyDefinition}. Valid values are: {string.Join(", ", enumValues)}");

			return enumValues[strValue];
		}

		public override object ConvertToPropertyBagValue(IPropertyDefinition propertyDefinition, object value, bool keepRelationInfo)
		{
			var strValue = value as string;
			if (strValue == null) return null;

			return strValue;
		}

		public override void ModelLoaded(IPropertyDefinition propertyDefinition)
		{
			var values = propertyDefinition.PropertyBag[PropertyValues] as string[];
			if((values == null) || (!values.Any()))
				throw new ModelLoadingException(
					$"An enum must have an {PropertyValues} property that is a string array of valid enum members (on {propertyDefinition}).");

			propertyDefinition.AdditionalProperties[PropertyValues] = values.ToDictionary(x => x, StringComparer.OrdinalIgnoreCase);
		}
	}
}