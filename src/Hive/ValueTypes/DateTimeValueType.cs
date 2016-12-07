using NodaTime;

namespace Hive.ValueTypes
{
	public class DateTimeValueType : ValueType<Instant>
	{
		public DateTimeValueType()
			: base("datetime")
		{
		}
	}
}