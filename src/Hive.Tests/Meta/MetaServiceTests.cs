﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Hive.Meta.Impl;
using Xunit;
using FluentAssertions;
using Hive.DependencyInjection;
using Hive.Handlers;
using Hive.Meta;
using Microsoft.Extensions.DependencyInjection;

namespace Hive.Tests.Meta
{
	public class MetaServiceTests
	{
		[Theory]
		[InlineData(typeof(User), "user", "users")]
		[InlineData(typeof(Man), "man", "men")]
		[InlineData(typeof(Custom), "Custom1", "Custom2")]
		public void ItShouldReturnResourceDescription(Type type, string expectedSingleName, string expectedPluralName)
		{
			var metaService = new MetaService(new ServiceCollection());
			var result = metaService.GetResourceDescription(type);

			result.SingleName.Should().Be(expectedSingleName);
			result.PluralName.Should().Be(expectedPluralName);
		}

		[Fact]
		public void ItShouldReturnHandlerInfos()
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddHandler<TestUserHandler>();
			serviceCollection.AddHandler<TestManHandler>();
			var metaService = new MetaService(serviceCollection);

			var result = metaService.GetHandlerInfos();

			result.Should().HaveCount(3);
			result
				.Should()
				.Contain(x => (x.HandlerType == HandlerTypes.Get)
							&& x.HandlerInterfaceType == typeof(IHandleGet<User>)
							&& x.ResourceDescription != null
							&& x.ResourceType == typeof(User))
				.And
				.Contain(x => (x.HandlerType == HandlerTypes.Delete)
							&& x.HandlerInterfaceType == typeof(IHandleDelete<User>)
							&& x.ResourceDescription != null
							&& x.ResourceType == typeof(User))
				.And
				.Contain(x => (x.HandlerType == HandlerTypes.Update)
							&& x.HandlerInterfaceType == typeof(IHandleUpdate<Man>)
							&& x.ResourceDescription != null
							&& x.ResourceType == typeof(Man));
		}

		private class User { }

		private class Man { }

		[Name(SingleName = "Custom1", PluralName = "Custom2")]
		private class Custom { }

		private class TestUserHandler : IHandleGet<User>, IHandleDelete<User>
		{
			public Task<IHandlerResult> Get(object id, CancellationToken ct)
			{
				throw new NotImplementedException();
			}

			public Task<IHandlerResult> Delete(User resource, CancellationToken ct)
			{
				throw new NotImplementedException();
			}
		}

		private class TestManHandler : IHandleUpdate<Man>
		{
			public Task<IHandlerResult> Update(Man resource, CancellationToken ct)
			{
				throw new NotImplementedException();
			}
		}
	}
}