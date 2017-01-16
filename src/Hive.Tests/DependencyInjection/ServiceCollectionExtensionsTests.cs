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

			serviceProvider.GetRequiredService<IHandleGet<TestEntity>>().Should().BeOfType<TestHandler>();
			serviceProvider.GetRequiredService<IHandleCreate<TestEntity>>().Should().BeOfType<TestHandler>();
		}

		[Fact]
		public void ItShouldRegisterAllEntityRepositoryHandlers()
		{
			var servicesCollections = new ServiceCollection();
			servicesCollections.AddEntityRepositoryHandlers<TestEntity>();

			var serviceProvider = servicesCollections.BuildServiceProvider(true);

			serviceProvider.GetRequiredService<IHandleGet<TestEntity>>().Should().BeOfType<EntityRepositoryGetHandler<TestEntity>>();
			serviceProvider.GetRequiredService<IHandleCreate<TestEntity>>().Should().BeOfType<EntityRepositoryCreateHandler<TestEntity>>();
			serviceProvider.GetRequiredService<IHandleUpdate<TestEntity>>().Should().BeOfType<EntityRepositoryUpdateHandler<TestEntity>>();
			serviceProvider.GetRequiredService<IHandleDelete<TestEntity>>().Should().BeOfType<EntityRepositoryDeleteHandler<TestEntity>>();
		}

		[Fact]
		public void ItShouldRegisterEntityRepositoryHandlersSelectively()
		{
			var servicesCollections = new ServiceCollection();
			servicesCollections.AddEntityRepositoryHandlers<TestEntity>(HandlerTypes.Create | HandlerTypes.Delete);

			var serviceProvider = servicesCollections.BuildServiceProvider(true);

			serviceProvider.GetRequiredService<IHandleCreate<TestEntity>>().Should().BeOfType<EntityRepositoryCreateHandler<TestEntity>>();
			serviceProvider.GetRequiredService<IHandleDelete<TestEntity>>().Should().BeOfType<EntityRepositoryDeleteHandler<TestEntity>>();

			serviceProvider.GetService<IHandleGet<TestEntity>>().Should().BeNull();
			serviceProvider.GetService<IHandleUpdate<TestEntity>>().Should().BeNull();
		}

		private class TestEntity : IEntity<int>
		{
			public int Id { get; }
		}

		private class TestHandler : IHandleGet<TestEntity>, IHandleCreate<TestEntity>
		{
			public Task<IHandlerResult> Get(object id, CancellationToken ct)
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