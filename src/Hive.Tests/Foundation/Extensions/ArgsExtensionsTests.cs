using System;
using FluentAssertions;
using Hive.Foundation.Extensions;
using Xunit;

namespace Hive.Tests.Foundation.Extensions
{
	public class ArgsExtensionsTests
	{
		[Theory]
		[InlineData(null, true)]
		[InlineData("", false)]
		public void NotNull(object input, bool shouldThrow)
		{
			if (shouldThrow)
				input.Invoking(x => x.NotNull(nameof(input))).ShouldThrow<ArgumentNullException>();
			else
				input.Invoking(x => x.NotNull(nameof(input))).ShouldNotThrow();
		}

		[Theory]
		[InlineData(null, true)]
		[InlineData("", true)]
		[InlineData("foo", false)]
		public void NotNullOrEmpty(string input, bool shouldThrow)
		{
			if (shouldThrow)
				input.Invoking(x => x.NotNullOrEmpty(nameof(input))).ShouldThrow<ArgumentNullException>();
			else
				input.Invoking(x => x.NotNullOrEmpty(nameof(input))).ShouldNotThrow();
		}
	}
}