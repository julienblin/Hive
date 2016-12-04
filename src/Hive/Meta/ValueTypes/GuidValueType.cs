using System;
using Hive.Exceptions;

namespace Hive.Meta.ValueTypes
{
	public class GuidValueType : ValueType<Guid>
	{
		public GuidValueType()
			: base("uuid")
		{
		}

		public override object ConvertValue(IPropertyDefinition propertyDefinition, object value)
		{
			if (value == null) return null;

			if (value is Guid) return value;

			Guid result;
			if (Guid.TryParse(value.ToString(), out result))
			{
				return result;
			}

			throw new ValueTypeException(this, $"Unable to parse value {value} as a valid UUID.");
		}
	}
}