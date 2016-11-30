﻿using System.Collections.Generic;
using FluentAssertions;
using Hive.Foundation.Entities;
using Xunit;

namespace Hive.Tests.Foundation.Entities
{
	public class SemVerTests
	{
		[Theory]
		[MemberData(nameof(ParseData))]
		public void Parse(string value, SemVer expected)
		{
			SemVer.Parse(value).ShouldBeEquivalentTo(expected);
		}

		public static IEnumerable<object[]> ParseData()
		{
			yield return new object[] { "1", new SemVer(1, 0, 0) };
			yield return new object[] { "1.2.3", new SemVer(1, 2, 3) };
			yield return new object[] { "0.1.2-pre12+13248", new SemVer(0, 1, 2, "pre12", "13248") };
		}

		[Theory]
		[MemberData(nameof(CompareData))]
		public void Compare(SemVer value1, object value2, int expected)
		{
			value1.CompareTo(value2).Should().Be(expected);
		}

		public static IEnumerable<object[]> CompareData()
		{
			yield return new object[] { new SemVer(1, 0, 0), new SemVer(1, 0, 0), 0 };
			yield return new object[] { new SemVer(1, 0, 0, prerelease: "pre"), new SemVer(1, 0, 0), -1 };
			yield return new object[] { new SemVer(1, 0, 0, build: "1234"), new SemVer(1, 0, 0), 1 };

			yield return new object[] { new SemVer(1, 0, 1), new SemVer(1, 0, 0), 1 };
			yield return new object[] { new SemVer(1, 1, 0), new SemVer(1, 0, 0), 1 };
			yield return new object[] { new SemVer(2, 0, 0), new SemVer(1, 0, 0), 1 };
			yield return new object[] { new SemVer(1, 0, 0), new SemVer(1, 0, 1), -1 };
			yield return new object[] { new SemVer(1, 0, 0), new SemVer(1, 1, 0), -1 };
			yield return new object[] { new SemVer(1, 0, 0), new SemVer(2, 0, 0), -1 };
		}
	}
}