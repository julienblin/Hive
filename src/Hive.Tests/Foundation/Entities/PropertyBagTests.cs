using System;
using System.Collections.Generic;
using FluentAssertions;
using Hive.Exceptions;
using Hive.Foundation.Entities;
using Newtonsoft.Json;
using Xunit;

namespace Hive.Tests.Foundation.Entities
{
	public class PropertyBagTests
	{
		[Fact]
		public void ItShouldBehaveLikeASafeDictionary()
		{
			var bag = new PropertyBag
			{
				["foo"] = 1,
				["bar"] = "foobar"
			};

			bag["foo"].Should().Be(1);
			bag["bar"].Should().Be("foobar");
			bag["unknown"].Should().BeNull();

			bag.Should().HaveCount(2);

			bag.Clear();
			bag["foo"].Should().BeNull();
		}

#if DEBUG

		[Fact]
		public void ItShouldCheckTypesInDebug()
		{
			var bag = new PropertyBag();

			Action act = () => bag["foo"] = "value";
			act.ShouldNotThrow();

			act = () => bag["foo"] = new PropertyBag();
			act.ShouldNotThrow<DebugException>();

			act = () => bag["foo"] = new[] { "string" };
			act.ShouldNotThrow<DebugException>();

			act = () => bag["foo"] = new[] { new[] { "string" } };
			act.ShouldNotThrow<DebugException>();

			act = () => bag["foo"] = DateTime.Now;
			act.ShouldThrow<DebugException>();

			act = () => bag["foo"] = new[] { DateTime.Now };
			act.ShouldThrow<DebugException>();
		}
#endif

		[Theory]
		[MemberData(nameof(JsonDeserializationData))]
		public void JsonDeserialization(string json, Action<PropertyBag> asserts)
		{
			var bag = JsonConvert.DeserializeObject<PropertyBag>(json);
			asserts(bag);
		}

		public static IEnumerable<object[]> JsonDeserializationData()
		{
			yield return new object[]
			{
				"{}",
				new Action<PropertyBag>(bag => bag.Should().NotBeNull()), 
			};

			yield return new object[]
			{
				"{ 'foo': 'bar', 'bar': 1 }",
				new Action<PropertyBag>(bag =>
				{
					bag["foo"].Should().Be("bar");
					bag["bar"].Should().Be(1);
				}),
			};

			yield return new object[]
			{
				"{ 'foo': ['bar', 'bar2'] }",
				new Action<PropertyBag>(bag =>
				{
					bag["foo"].ShouldBeEquivalentTo(new[] { "bar", "bar2" });
				}),
			};

			yield return new object[]
			{
				"{ 'foo': [['bar', 'bar2']] }",
				new Action<PropertyBag>(bag =>
				{
					bag["foo"].ShouldBeEquivalentTo(new[] { new[] { "bar", "bar2" } });
				}),
			};

			yield return new object[]
			{
				"{ 'foo': { 'bar': 'foobar' } }",
				new Action<PropertyBag>(bag =>
				{
					bag["foo"].ShouldBeEquivalentTo(new PropertyBag { ["bar"] = "foobar" });
				}),
			};
		}

		[Theory]
		[MemberData(nameof(JsonSerializationData))]
		public void JsonSerialization(PropertyBag bag, string expected)
		{
			var result = JsonConvert.SerializeObject(bag);
			result.ShouldBeEquivalentTo(expected);
		}

		public static IEnumerable<object[]> JsonSerializationData()
		{
			yield return new object[]
			{
				new PropertyBag(),
				"{}" 
			};

			yield return new object[]
			{
				new PropertyBag
				{
					["foo"] = "bar",
					["bar"] = 1
				},
				"{\"foo\":\"bar\",\"bar\":1}"
			};

			yield return new object[]
			{
				new PropertyBag
				{
					["foo"] = new[] { "bar" },
				},
				"{\"foo\":[\"bar\"]}"
			};

			yield return new object[]
			{
				new PropertyBag
				{
					["foo"] = new PropertyBag
					{
						["bar"] = 1
					},
				},
				"{\"foo\":{\"bar\":1}}"
			};
		}
	}
}
 