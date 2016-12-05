using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta.Data;
using Hive.Meta.Impl;

namespace Hive.Meta.ValueTypes
{
	public abstract class ValueType<T> : IValueType
	{
		protected ValueType(string name)
		{
			Name = name.NotNullOrEmpty(nameof(name));
		}

		public string Name { get; }

		public Type InternalNetType => typeof(T);

		public virtual void FinishLoading(IValueTypeFactory valueTypeFactory, IPropertyDefinition propertyDefinition)
		{
			var propertyOriginalData = propertyDefinition as IOriginalDataHolder<PropertyDefinitionData>;
			if (propertyOriginalData == null) return;

			var defaultValue = propertyOriginalData.OriginalData?.GetValue<object>(MetaConstants.DefaultProperty);
			if (defaultValue != null)
			{
				propertyDefinition.SetProperty(MetaConstants.DefaultProperty, defaultValue);
			}
		}

		public virtual object ConvertValue(IPropertyDefinition propertyDefinition, object value)
		{
			return value;
		}

		public virtual Task SetDefaultValue(IPropertyDefinition propertyDefinition, IEntity entity, CancellationToken ct)
		{
			if (propertyDefinition.DefaultValue == null) return Task.CompletedTask;

			entity.SetPropertyValue(propertyDefinition.Name, propertyDefinition.DefaultValue);

			return Task.CompletedTask;
		}
	}
}