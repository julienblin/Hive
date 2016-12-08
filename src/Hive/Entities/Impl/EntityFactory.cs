using System;
using System.Threading;
using System.Threading.Tasks;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Entities.Impl
{
	public class EntityFactory : IEntityFactory
	{
		public async Task<IEntity> Create(IEntityDefinition definition, CancellationToken ct)
		{
			definition.NotNull(nameof(definition));
			var entity = Hydrate(definition);
			await Init(entity, ct);
			return entity;
		}

		public async Task<IEntity> Create(IEntityDefinition definition, PropertyBag initialValues, CancellationToken ct)
		{
			definition.NotNull(nameof(definition));
			var entity = Hydrate(definition, initialValues);
			await Init(entity, ct);
			return entity;
		}

		public Task<IEntity> Hydrate(IEntityDefinition definition, PropertyBag initialValues, CancellationToken ct)
		{
			definition.NotNull(nameof(definition));
			return Task.FromResult(Hydrate(definition, initialValues));
		}

		private static IEntity Hydrate(IEntityDefinition definition, PropertyBag propertyBag = null)
		{
			var entity = new Entity(definition);
			if (propertyBag != null)
			{
				foreach (var property in propertyBag)
				{
					var propertyDefinition = definition.Properties.SafeGet(property.Key);
					if (propertyDefinition != null)
						entity[property.Key] = propertyDefinition.PropertyType.ConvertFromPropertyBagValue(propertyDefinition, property.Value);
				}
			}
			return entity;
		}

		private static async Task Init(IEntity entity, CancellationToken ct)
		{
			foreach (var propertyDefinition in entity.Definition.Properties.Values)
				if ((propertyDefinition.DefaultValue != null) && (entity[propertyDefinition.Name] == null))
					await propertyDefinition.SetDefaultValue(entity, ct);
		}
	}
}