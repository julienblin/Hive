using NodaTime;

namespace Hive.Meta.ValueTypes
{
	public class DateTimeValueType : ValueType<Instant>
	{
		public DateTimeValueType()
			: base("datetime")
		{
		}
	}
}