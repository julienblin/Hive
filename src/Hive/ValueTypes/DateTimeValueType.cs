using System;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Exceptions;
using Hive.Foundation.Extensions;
using Hive.Meta;
using NodaTime;
using NodaTime.Text;
using NodaTime.TimeZones;

namespace Hive.ValueTypes
{
	public class DateTimeValueType : ValueType<Instant>
	{
		private const string NowDefaultValue = "now()";

		private static readonly OffsetDateTimePattern ParsePattern = OffsetDateTimePattern.GeneralIsoPattern;

		public DateTimeValueType()
			: base("datetime")
		{
		}

		public override object ConvertFromPropertyBagValue(IPropertyDefinition propertyDefinition, object value)
		{
			if (value == null) return null;

			if (value is int)
			{
				return Instant.FromUnixTimeSeconds((int) value);
			}

			if (value is long)
			{
				return Instant.FromUnixTimeSeconds((long)value);
			}

			if (value is string)
			{
				var parseResult = ParsePattern.Parse((string)value);
				if (parseResult.Success)
					return parseResult.Value.ToInstant();

				throw new ValueTypeException(this,
					$"Error while parsing value {value} as a datetime using pattern {ParsePattern.PatternText}.",
					parseResult.Exception);
			}

			throw new ValueTypeException(this, $"Unable to convert value {value} to a datetime.");
		}

		public override object ConvertToPropertyBagValue(IPropertyDefinition propertyDefinition, object value, bool keepRelationInfo)
		{
			if (value == null) return null;

			if (value is Instant)
				return InstantPattern.ExtendedIsoPattern.Format((Instant)value);

			throw new NotSupportedException();
		}

		public override Task SetDefaultValue(IPropertyDefinition propertyDefinition, IEntity entity, CancellationToken ct)
		{
			var defaultValue = propertyDefinition.DefaultValue as string;
			if (defaultValue.IsNullOrEmpty()) return Task.CompletedTask;

			if (defaultValue.SafeOrdinalEquals(NowDefaultValue))
				entity[propertyDefinition.Name] = SystemClock.Instance.GetCurrentInstant();
			else
			{
				var parseResult = ParsePattern.Parse(defaultValue);
				if (parseResult.Success)
					entity[propertyDefinition.Name] = parseResult.Value;
				else
					throw new ValueTypeException(this,
					$"Error while parsing value {defaultValue} as a datetime using pattern {ParsePattern.PatternText}.",
					parseResult.Exception);
			}
				

			return Task.CompletedTask;
		}

		public override DataTypeType DataTypeType => DataTypeType.DateTime;
	}
}