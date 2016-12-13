using System;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Exceptions;
using Hive.Foundation.Extensions;
using Hive.Meta;
using NodaTime;
using NodaTime.Text;

namespace Hive.ValueTypes
{
	public class DateValueType : ValueType<LocalDate>
	{
		//TODO: allow parsing of timezones.
		private const string TodayDefaultValue = "today()";

		public DateValueType()
			: base("date")
		{
		}

		public override object ConvertToPropertyBagValue(IPropertyDefinition propertyDefinition, object value, bool keepRelationInfo)
		{
			if (value == null) return null;

			if (value is LocalDate)
				return LocalDatePattern.IsoPattern.Format((LocalDate) value);

			throw new NotSupportedException();
		}

		public override object ConvertFromPropertyBagValue(IPropertyDefinition propertyDefinition, object value)
		{
			var strValue = value as string;
			if (strValue == null) return null;

			var parseResult = LocalDatePattern.IsoPattern.Parse(strValue);
			if (parseResult.Success)
				return parseResult.Value;

			throw new ValueTypeException(this,
				$"Error while parsing value {value} as a local date using pattern {LocalDatePattern.IsoPattern.PatternText}.",
				parseResult.Exception);
		}

		public override Task SetDefaultValue(IPropertyDefinition propertyDefinition, IEntity entity, CancellationToken ct)
		{
			var defaultValue = propertyDefinition.DefaultValue as string;
			if (defaultValue.IsNullOrEmpty()) return Task.CompletedTask;

			if (defaultValue.SafeOrdinalEquals(TodayDefaultValue))
				entity[propertyDefinition.Name] = SystemClock.Instance.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb.GetSystemDefault()).Date;
			else
			{
				var parseResult = LocalDatePattern.IsoPattern.Parse(defaultValue);
				if (parseResult.Success)
					entity[propertyDefinition.Name] = parseResult.Value;
				else
					throw new ValueTypeException(this,
					$"Error while parsing value {defaultValue} as a date using pattern {LocalDatePattern.IsoPattern.PatternText}.",
					parseResult.Exception);
			}


			return Task.CompletedTask;
		}

		public override DataTypeType DataTypeType => DataTypeType.Date;
	}
}