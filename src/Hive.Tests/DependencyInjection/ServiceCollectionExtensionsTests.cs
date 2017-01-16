using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hive.DependencyInjection;
using Hive.Handlers;
using Hive.Handlers.Impl;
using Hive.Entities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Hive.Tests.DependencyInjection
{
	public class ServiceCollectionExtensionsTests
	{
		[Fact]
		public void ItShouldRegisterHandler()
		{
			var servicesCollections = new ServiceCollection();
			servicesCollections.AddHandler<TestHandler>();

			var serviceProvider = servicesCollections.BuildServiceProvider(true);

			serviceProvider.GetRequiredService<IHandleGet<TestEntity, int>>().Should().BeOfType<TestHandler>();
			serviceProvider.GetRequiredService<IHandleCreate<TestEntity>>().Should().BeOfType<TestHandler>();
		}

		[Fact]
		public void ItShouldRegisterAllEntityRepositoryHandlers()
		{
			var servicesCollections = new ServiceCollection();
			servicesCollections.AddEntityRepositoryHandlers<TestEntity, int>();

			var serviceProvider = servicesCollections.BuildServiceProvider(true);

			serviceProvider.GetRequiredService<IHandleGet<TestEntity, int>>().Should().BeOfType<EntityRepositoryGetHandler<TestEntity, int>>();
			serviceProvider.GetRequiredService<IHandleCreate<TestEntity>>().Should().BeOfType<EntityRepositoryCreateHandler<TestEntity, int>>();
			serviceProvider.GetRequiredService<IHandleUpdate<TestEntity>>().Should().BeOfType<EntityRepositoryUpdateHandler<TestEntity, int>>();
			serviceProvider.GetRequiredService<IHandleDelete<TestEntity, int>>().Should().BeOfType<EntityRepositoryDeleteHandler<TestEntity, int>>();
		}

		[Fact]
		public void ItShouldRegisterEntityRepositoryHandlersSelectively()
		{
			var servicesCollections = new ServiceCollection();
			servicesCollections.AddEntityRepositoryHandlers<TestEntity, int>(HandlerTypes.Create | HandlerTypes.Delete);

			var serviceProvider = servicesCollections.BuildServiceProvider(true);

			serviceProvider.GetRequiredService<IHandleCreate<TestEntity>>().Should().BeOfType<EntityRepositoryCreateHandler<TestEntity, int>>();
			serviceProvider.GetRequiredService<IHandleDelete<TestEntity, int>>().Should().BeOfType<EntityRepositoryDeleteHandler<TestEntity, int>>();

			serviceProvider.GetService<IHandleGet<TestEntity, int>>().Should().BeNull();
			serviceProvider.GetService<IHandleUpdate<TestEntity>>().Should().BeNull();
		}

		private class TestEntity : IEntity<int>
		{
			public int Id { get; }
		}

		private class TestHandler : IHandleGet<TestEntity, int>, IHandleCreate<TestEntity>
		{
			public Task<IHandlerResult> Get(int id, CancellationToken ct)
			{
				throw new System.NotImplementedException();
			}

			public Task<IHandlerResult> Create(TestEntity resource, CancellationToken ct)
			{
				throw new System.NotImplementedException();
			}
		}
	}
}