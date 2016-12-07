using System;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.Meta.Data;

namespace Hive.ValueTypes
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

		public virtual object ConvertToPropertyBagValue(IPropertyDefinition propertyDefinition, object value)
		{
			return value;
		}

		public virtual object ConvertFromPropertyBagValue(IPropertyDefinition propertyDefinition, object value)
		{
			return value;
		}

		public virtual Task SetDefaultValue(IPropertyDefinition propertyDefinition, IEntity entity, CancellationToken ct)
		{
			if (propertyDefinition.DefaultValue == null) return Task.CompletedTask;

			entity[propertyDefinition.Name] = propertyDefinition.DefaultValue;

			return Task.CompletedTask;
		}
	}
}