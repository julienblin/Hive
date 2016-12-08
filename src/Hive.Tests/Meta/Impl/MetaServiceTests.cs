using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hive.Foundation.Entities;
using Hive.Meta.Impl;
using Hive.Tests.Mocks;
using Xunit;

namespace Hive.Tests.Meta.Impl
{
	public class MetaServiceTests
	{
		[Fact]
		public async Task ItShouldGetModelFromCache()
		{
			var cachedModel = new ModelMock();

			var metaService = new MetaService(
				new MetaRepositoryMock(),
				new ModelLoaderMock(),
				new ModelCacheMock(cachedModel)
			);

			var result = await metaService.GetModel(cachedModel.Name, CancellationToken.None);
			result.Should().BeSameAs(cachedModel);
		}

		[Fact]
		public async Task ItShouldLoadModelIfNotFromCache()
		{
			var model = new ModelMock();

			var metaService = new MetaService(
				new MetaRepositoryMock(new PropertyBag()),
				new ModelLoaderMock(model),
				new ModelCacheMock(putAsserts: m => { m.Should().BeSameAs(model); })
			);

			var result = await metaService.GetModel(model.Name, CancellationToken.None);
			result.Should().BeSameAs(model);
		}
	}
}