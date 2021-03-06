﻿using System;
using System.Collections.Immutable;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Entities.Impl;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Handlers;
using Hive.ValueTypes;

namespace Hive.Meta.Impl
{
	internal class EntityDefinition : IEntityDefinition
	{
		public PropertyBag PropertyBag { get; set; }

		public string FullName => $"{Model.Name}.{Name}";

		public string Name => SingleName;

		public Type InternalNetType => typeof(IEntity);

		public IModel Model { get; set; }

		public string SingleName { get; set; }

		public string PluralName { get; set; }

		public EntityType EntityType { get; set; }

		public IImmutableDictionary<string, IPropertyDefinition> Properties { get; set; }

		public ConcurrencyHandling ConcurrencyHandling { get; set; }

		public IHandler<IEntity, IEntity> CreateHandler { get; set; }

		public IHandler<IEntity, IEntity> UpdateHandler { get; set; }

		public IHandler<DeleteExecution, bool> DeleteHandler { get; set; }

		void IDataType.ModelLoaded(IPropertyDefinition propertyDefinition)
		{
		}

		object IDataType.ConvertToPropertyBagValue(IPropertyDefinition propertyDefinition, object value, bool keepRelationInfo)
		{
			var entityValue = value as IEntity;
			return entityValue?.ToPropertyBag(keepRelationInfo);
		}

		object IDataType.ConvertFromPropertyBagValue(IPropertyDefinition propertyDefinition, object value)
		{
			var propertyBagValue = value as PropertyBag;
			if (propertyBagValue == null) return null;

			return Model.Factories.Entity.Hydrate(this, propertyBagValue);
		}

		Task IDataType.SetDefaultValue(IPropertyDefinition propertyDefinition, IEntity entity, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		internal void ModelLoaded() => Properties?.Values.SafeForEach(x => ((PropertyDefinition)x).ModelLoaded());

		public DataTypeType DataTypeType => DataTypeType.Entity;

		public IDataType GetTargetValueType(IPropertyDefinition propertyDefinition) => null;

		public override string ToString() => FullName;
	}
}