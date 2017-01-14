using System;
using System.Collections.Generic;
using System.Reflection;
using Hive.Handlers;
using Hive.Validation;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using Hive.Handlers.Impl;
using Hive.Entities;
using Hive.Meta;
using Hive.Meta.Impl;

namespace Hive.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{

		public static IServiceCollection AddHive(this IServiceCollection serviceCollection)
		{
			if(!serviceCollection.Any(x => x.ServiceType == typeof(IServiceCollection)))
				serviceCollection.Add(new ServiceDescriptor(typeof(IServiceCollection), serviceCollection));

			serviceCollection.AddSingleton<IMetaService, MetaService>();

			return serviceCollection;
		}

		public static IServiceCollection AddHandler<T>(this IServiceCollection serviceCollection, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
			where T : class, IHandler
		{
			var typeInterfaces = typeof(T).GetInterfaces().Select(x => x.GetTypeInfo()).ToList();

			AddHandler<T>(serviceCollection, typeInterfaces, typeof(IHandleGet<>), serviceLifetime);
			AddHandler<T>(serviceCollection, typeInterfaces, typeof(IHandleCreate<>), serviceLifetime);
			AddHandler<T>(serviceCollection, typeInterfaces, typeof(IHandleUpdate<>), serviceLifetime);
			AddHandler<T>(serviceCollection, typeInterfaces, typeof(IHandleDelete<>), serviceLifetime);

			return serviceCollection;
		}

		public static IServiceCollection AddDefaultHandlers<T>(this IServiceCollection serviceCollection)
			where T : class, IEntity
		{
			return AddDefaultHandlers<T>(serviceCollection, HandlerTypes.All);
		}

		public static IServiceCollection AddDefaultHandlers<T>(this IServiceCollection serviceCollection, HandlerTypes handlerTypes)
			where T : class, IEntity
		{
			if (handlerTypes.HasFlag(HandlerTypes.Get))
			{
				serviceCollection.Add(
					new ServiceDescriptor(
						typeof(IHandleGet<>).MakeGenericType(typeof(T)),
						typeof(DefaultModelGetHandler<>).MakeGenericType(typeof(T)),
						ServiceLifetime.Singleton
					)
				);
			}

			if (handlerTypes.HasFlag(HandlerTypes.Create))
			{
				serviceCollection.Add(
					new ServiceDescriptor(
						typeof(IHandleCreate<>).MakeGenericType(typeof(T)),
						typeof(DefaultModelCreateHandler<>).MakeGenericType(typeof(T)),
						ServiceLifetime.Singleton
					)
				);
			}

			if (handlerTypes.HasFlag(HandlerTypes.Update))
			{
				serviceCollection.Add(
					new ServiceDescriptor(
						typeof(IHandleUpdate<>).MakeGenericType(typeof(T)),
						typeof(DefaultModelUpdateHandler<>).MakeGenericType(typeof(T)),
						ServiceLifetime.Singleton
					)
				);
			}

			if (handlerTypes.HasFlag(HandlerTypes.Delete))
			{
				serviceCollection.Add(
					new ServiceDescriptor(
						typeof(IHandleDelete<>).MakeGenericType(typeof(T)),
						typeof(DefaultModelDeleteHandler<>).MakeGenericType(typeof(T)),
						ServiceLifetime.Singleton
					)
				);
			}

			return serviceCollection;
		}

		private static void AddHandler<TImplementation>(
			IServiceCollection serviceCollection,
			IEnumerable<TypeInfo> typeInterfaces,
			Type handlerType,
			ServiceLifetime serviceLifetime)
			where TImplementation : class, IHandler
		{
			var handlerInterface =
				typeInterfaces.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == handlerType);
			if (handlerInterface != null)
			{
				serviceCollection.Add(new ServiceDescriptor(handlerInterface.AsType(), typeof(TImplementation), serviceLifetime));
			}
		}
	}
}