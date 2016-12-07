using System;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Exceptions;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.ValueTypes
{
	public class GuidValueType : ValueType<Guid>
	{
		internal const string NewGuidDefaultValue = "new()";

		public GuidValueType()
			: base("uuid")
		{
		}

		public override object ConvertFrom(IPropertyDefinition propertyDefinition, object value)
		{
			if (value == null) return null;

			if (value is Guid) return value;

			if ((value is string) && string.IsNullOrEmpty((string) value)) return null;

			Guid result;
			if (Guid.TryParse(value.ToString(), out result))
			{
				return result;
			}

			throw new ValueTypeException(this, $"Unable to parse value {value} as a valid UUID.");
		}

		public override Task SetDefaultValue(IPropertyDefinition propertyDefinition, IEntity entity, CancellationToken ct)
		{
			var defaultValue = propertyDefinition.DefaultValue as string;
			if (defaultValue.IsNullOrEmpty()) return Task.CompletedTask;

			if (defaultValue.SafeOrdinalEquals(NewGuidDefaultValue))
			{
				entity.SetPropertyValue(propertyDefinition.Name, Guid.NewGuid());
			}
			else
			{
				entity.SetPropertyValue(propertyDefinition.Name, defaultValue);
			}

			return Task.CompletedTask;
		}
	}
}