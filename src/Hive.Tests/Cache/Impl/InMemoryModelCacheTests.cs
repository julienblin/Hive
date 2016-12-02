using FluentAssertions;
using Hive.Cache.Impl;
using Hive.Tests.Mocks;
using Xunit;

namespace Hive.Tests.Cache.Impl
{
	public class InMemoryModelCacheTests
	{
		[Fact]
		public void ItShouldCache()
		{
			var cache = new InMemoryModelCache();
			var model = new ModelMock();

			cache.Put(model);

			cache.Get(model.Name).Should().BeSameAs(model);

			cache.Get("Foo").Should().BeNull();
		}
	}
}