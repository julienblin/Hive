using System;
using Hive.Exceptions;
using Hive.Meta;
using NodaTime;
using NodaTime.Text;

namespace Hive.ValueTypes
{
	public class DateValueType : ValueType<LocalDate>
	{
		public DateValueType()
			: base("date")
		{
		}

		public override object ConvertToPropertyBagValue(IPropertyDefinition propertyDefinition, object value)
		{
			if (value == null) return null;

			if (value is LocalDate)
				return LocalDatePattern.IsoPattern.Format((LocalDate) value);

			throw new NotSupportedException();
		}

		public override object ConvertFromPropertyBagValue(IPropertyDefinition propertyDefinition, object value)
		{
			if (value == null) return null;

			if (value is LocalDate) return value;

			if (value is DateTime)
			{
				var realValue = (DateTime) value;
				return new LocalDate(realValue.Year, realValue.Month, realValue.Day);
			}

			if (value is DateTimeOffset)
			{
				var realValue = (DateTimeOffset) value;

				return new LocalDate(realValue.Year, realValue.Month, realValue.Day);
			}

			var parseResult = LocalDatePattern.IsoPattern.Parse(value.ToString());
			if (parseResult.Success)
				return parseResult.Value;

			throw new ValueTypeException(this,
				$"Error while parsing value {value} as a local date using pattern {LocalDatePattern.IsoPattern.PatternText}.",
				parseResult.Exception);
		}
	}
}