using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hive.DependencyInjection;
using Hive.Foundation.Extensions;
using Hive.Handlers;
using Humanizer;
using Microsoft.Extensions.DependencyInjection;

namespace Hive.Meta.Impl
{
	public class MetaService : IMetaService
	{
		private readonly IServiceCollection _serviceCollection;
		private readonly Lazy<IEnumerable<HandlerInfo>> _handlerInfos;
		private readonly ConcurrentDictionary<Type, ResourceDescription> _resourceDescriptions = new ConcurrentDictionary<Type, ResourceDescription>();

		public MetaService(IServiceCollection serviceCollection)
		{
			_serviceCollection = serviceCollection.NotNull(nameof(serviceCollection));
			_handlerInfos = new Lazy<IEnumerable<HandlerInfo>>(() => CreateHandlerInfos().ToList());
		}

		public IEnumerable<HandlerInfo> GetHandlerInfos()
		{
			return _handlerInfos.Value;
		}

		public ResourceDescription GetResourceDescription(Type type)
		{
			return _resourceDescriptions.GetOrAdd(type, CreateResourceDescription);
		}

		private IEnumerable<HandlerInfo> CreateHandlerInfos()
		{
			var handlerInterfaces = FindHandlerTypeInfos();
			foreach (var handlerInterface in handlerInterfaces)
			{
				yield return new HandlerInfo(
					handlerInterface.AsType(),
					handlerInterface.GetGenericArguments()[0],
					GetResourceDescription(handlerInterface.GetGenericArguments()[0]),
					GetHandlerType(handlerInterface)
				);
			}
		}

		private HandlerTypes GetHandlerType(TypeInfo handlerInterfaceTypeInfo)
		{
			var genericHandlerInterface = handlerInterfaceTypeInfo.GetGenericTypeDefinition();
			if (genericHandlerInterface == typeof(IHandleGet<>))
			{
				return HandlerTypes.Get;
			}

			if (genericHandlerInterface == typeof(IHandleCreate<>))
			{
				return HandlerTypes.Create;
			}

			if (genericHandlerInterface == typeof(IHandleUpdate<>))
			{
				return HandlerTypes.Update;
			}

			if (genericHandlerInterface == typeof(IHandleDelete<>))
			{
				return HandlerTypes.Delete;
			}

			throw new NotSupportedException($"Unknown handler interface type {handlerInterfaceTypeInfo}.");
		}

		private IEnumerable<TypeInfo> FindHandlerTypeInfos()
		{
			return _serviceCollection
				.Select(x => x.ServiceType.GetTypeInfo())
				.Where(x => x.IsGenericType)
				.Select(x => new { handlerInterfaceTypeInfo = x, handlerTypeGenericDefinition = x.GetGenericTypeDefinition()})
				.Where(x => (x.handlerTypeGenericDefinition == typeof(IHandleGet<>))
				         || (x.handlerTypeGenericDefinition == typeof(IHandleCreate<>))
				         || (x.handlerTypeGenericDefinition == typeof(IHandleUpdate<>))
				         || (x.handlerTypeGenericDefinition == typeof(IHandleDelete<>)))
				.Select(x => x.handlerInterfaceTypeInfo)
				.ToList();
		}

		private static ResourceDescription CreateResourceDescription(Type type)
		{
			var typeInfo = type.GetTypeInfo();
			var nameAttribute = typeInfo.GetCustomAttribute<NameAttribute>();
			var singleName = nameAttribute?.SingleName ?? typeInfo.Name.Singularize(false).Camelize();
			var pluralName = nameAttribute?.PluralName ?? typeInfo.Name.Pluralize(true).Camelize();

			return new ResourceDescription(singleName, pluralName);
		}
	}
}