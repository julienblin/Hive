using NodaTime;
using NodaTime.Text;

namespace Hive.Foundation.Extensions
{
	public static class InstantExtensions
	{
		public static string ToUtcIso8601(this Instant instant)
		{
			return InstantPattern.GeneralPattern.Format(instant);
		}
	}
}