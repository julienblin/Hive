using FluentAssertions;
using Hive.Foundation.Extensions;
using Xunit;

namespace Hive.Tests.Foundation.Extensions
{
	public class StringExtensionsTests
	{
		[Theory]
		[InlineData(null, null, true)]
		[InlineData(null, "", false)]
		[InlineData(null, "", false)]
		[InlineData("", "", true)]
		[InlineData("a", "a", true)]
		[InlineData("a", "b", false)]
		public void SafeOrdinalEquals(string value1, string value2, bool expected)
		{
			value1.SafeOrdinalEquals(value2).Should().Be(expected);
		}

		[Theory]
		[InlineData(null, null, null)]
		[InlineData("foo", "{0}bar", "foobar")]
		[InlineData(null, "{0}bar", null)]
		[InlineData("", "{0}bar", null)]
		[InlineData("foo", "bar", "bar")]
		public void SafeInvariantFormat(string value, string format, string expected)
		{
			value.SafeInvariantFormat(format).Should().BeEquivalentTo(expected);
		}

		[Theory]
		[InlineData("4", 4)]
		[InlineData("", null)]
		[InlineData(null, null)]
		[InlineData("foo", null)]
		public void IntSafeInvariantParse(string value, int? expected)
		{
			value.IntSafeInvariantParse().Should().Be(expected);
		}
	}
}