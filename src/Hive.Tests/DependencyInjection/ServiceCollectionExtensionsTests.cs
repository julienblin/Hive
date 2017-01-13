using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Hive.DependencyInjection;
using Hive.Handlers;
using Hive.Handlers.Impl;
using Hive.Models;
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

			serviceProvider.GetRequiredService<IHandleGet<TestModel>>().Should().BeOfType<TestHandler>();
			serviceProvider.GetRequiredService<IHandleCreate<TestModel>>().Should().BeOfType<TestHandler>();
		}

		[Fact]
		public void ItShouldRegisterAllDefaultHandlers()
		{
			var servicesCollections = new ServiceCollection();
			servicesCollections.AddDefaultHandlers<TestModel>();

			var serviceProvider = servicesCollections.BuildServiceProvider(true);

			serviceProvider.GetRequiredService<IHandleGet<TestModel>>().Should().BeOfType<DefaultModelGetHandler<TestModel>>();
			serviceProvider.GetRequiredService<IHandleCreate<TestModel>>().Should().BeOfType<DefaultModelCreateHandler<TestModel>>();
			serviceProvider.GetRequiredService<IHandleUpdate<TestModel>>().Should().BeOfType<DefaultModelUpdateHandler<TestModel>>();
			serviceProvider.GetRequiredService<IHandleDelete<TestModel>>().Should().BeOfType<DefaultModelDeleteHandler<TestModel>>();
		}

		[Fact]
		public void ItShouldRegisterDefaultHandlersSelectively()
		{
			var servicesCollections = new ServiceCollection();
			servicesCollections.AddDefaultHandlers<TestModel>(HandlerTypes.Create | HandlerTypes.Delete);

			var serviceProvider = servicesCollections.BuildServiceProvider(true);

			serviceProvider.GetRequiredService<IHandleCreate<TestModel>>().Should().BeOfType<DefaultModelCreateHandler<TestModel>>();
			serviceProvider.GetRequiredService<IHandleDelete<TestModel>>().Should().BeOfType<DefaultModelDeleteHandler<TestModel>>();

			serviceProvider.GetService<IHandleGet<TestModel>>().Should().BeNull();
			serviceProvider.GetService<IHandleUpdate<TestModel>>().Should().BeNull();
		}

		private class TestModel : IModel<int>
		{
			public int Id { get; }
		}

		private class TestHandler : IHandleGet<TestModel>, IHandleCreate<TestModel>
		{
			public Task<IHandlerResult> Get(object id, CancellationToken ct)
			{
				throw new System.NotImplementedException();
			}

			public Task<IHandlerResult> Create(TestModel resource, CancellationToken ct)
			{
				throw new System.NotImplementedException();
			}
		}
	}
}