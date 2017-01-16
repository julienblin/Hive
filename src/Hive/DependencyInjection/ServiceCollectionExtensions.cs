using System;
using System.Collections.Generic;
using System.Reflection;
using Hive.Handlers;
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
			where T : class
		{
			var typeInterfaces = typeof(T).GetInterfaces().Select(x => x.GetTypeInfo()).ToList();

			AddHandler<T>(serviceCollection, typeInterfaces, typeof(IHandleGet<,>), serviceLifetime);
			AddHandler<T>(serviceCollection, typeInterfaces, typeof(IHandleCreate<>), serviceLifetime);
			AddHandler<T>(serviceCollection, typeInterfaces, typeof(IHandleUpdate<>), serviceLifetime);
			AddHandler<T>(serviceCollection, typeInterfaces, typeof(IHandleDelete<,>), serviceLifetime);

			return serviceCollection;
		}

		public static IServiceCollection AddEntityRepositoryHandlers<TEntity, TId>(this IServiceCollection serviceCollection)
			where TEntity : class, IEntity<TId>
		{
			return AddEntityRepositoryHandlers<TEntity, TId>(serviceCollection, HandlerTypes.All);
		}

		public static IServiceCollection AddEntityRepositoryHandlers<TEntity, TId>(this IServiceCollection serviceCollection, HandlerTypes handlerTypes)
			where TEntity : class, IEntity<TId>
		{
			if (handlerTypes.HasFlag(HandlerTypes.Get))
			{
				serviceCollection.Add(
					new ServiceDescriptor(
						typeof(IHandleGet<,>).MakeGenericType(typeof(TEntity), typeof(TId)),
						typeof(EntityRepositoryGetHandler<,>).MakeGenericType(typeof(TEntity), typeof(TId)),
						ServiceLifetime.Singleton
					)
				);
			}

			if (handlerTypes.HasFlag(HandlerTypes.Create))
			{
				serviceCollection.Add(
					new ServiceDescriptor(
						typeof(IHandleCreate<>).MakeGenericType(typeof(TEntity)),
						typeof(EntityRepositoryCreateHandler<,>).MakeGenericType(typeof(TEntity), typeof(TId)),
						ServiceLifetime.Singleton
					)
				);
			}

			if (handlerTypes.HasFlag(HandlerTypes.Update))
			{
				serviceCollection.Add(
					new ServiceDescriptor(
						typeof(IHandleUpdate<>).MakeGenericType(typeof(TEntity)),
						typeof(EntityRepositoryUpdateHandler<,>).MakeGenericType(typeof(TEntity), typeof(TId)),
						ServiceLifetime.Singleton
					)
				);
			}

			if (handlerTypes.HasFlag(HandlerTypes.Delete))
			{
				serviceCollection.Add(
					new ServiceDescriptor(
						typeof(IHandleDelete<,>).MakeGenericType(typeof(TEntity), typeof(TId)),
						typeof(EntityRepositoryDeleteHandler<,>).MakeGenericType(typeof(TEntity), typeof(TId)),
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
			where TImplementation : class
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