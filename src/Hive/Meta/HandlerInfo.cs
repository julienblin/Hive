using System;
using Hive.DependencyInjection;
using Hive.Foundation.Extensions;

namespace Hive.Meta
{
	public class HandlerInfo
	{
		public HandlerInfo(
			Type handlerInterfaceType,
			Type resourceType,
			Type knownIdType,
			ResourceDescription resourceDescription,
			HandlerTypes handlerType)
		{
			HandlerInterfaceType = handlerInterfaceType.NotNull(nameof(handlerInterfaceType));
			ResourceType = resourceType.NotNull(nameof(resourceType));
			KnownIdType = knownIdType;
			ResourceDescription = resourceDescription.NotNull(nameof(resourceDescription));
			HandlerType = handlerType;
		}

		public Type HandlerInterfaceType { get; }

		public Type ResourceType { get; }

		public Type KnownIdType { get; }

		public ResourceDescription ResourceDescription { get; }

		public HandlerTypes HandlerType { get; }
	}
}