using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentAssertions;
using Hive.Foundation.Extensions;
using Hive.Foundation.Validation;
using Xunit;
using ValidationException = Hive.Exceptions.ValidationException;

namespace Hive.Tests.Foundation.Validation
{
	public class ObjectValidationTests
	{
		private class ClassToTest
		{
			[Required]
			public string Name { get; set; }
		}

		[Fact]
		public void TryValidate()
		{
			var test = new ClassToTest();
			var results = test.TryValidate();
			results.IsValid.Should().BeFalse();
			results.Errors.Should().HaveCount(1);
			results.Errors.First().Target.Should().Be(nameof(ClassToTest.Name));

			test.Name = "Foo";
			results = test.TryValidate();
			results.IsValid.Should().BeTrue();
			results.Errors.Safe().Should().HaveCount(0);
		}

		[Fact]
		public void Validate()
		{
			var test = new ClassToTest();
			test.Invoking(x => x.Validate()).ShouldThrow<ValidationException>()
				.Which.Results.Errors.First().Target.Should().Be(nameof(ClassToTest.Name));
		}
	}
}