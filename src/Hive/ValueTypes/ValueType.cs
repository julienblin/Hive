using System;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;

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

		public virtual void ModelLoaded(IPropertyDefinition propertyDefinition)
		{
		}

		public virtual object ConvertToPropertyBagValue(IPropertyDefinition propertyDefinition, object value, bool keepRelationInfo)
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

		public virtual DataTypeType DataTypeType => DataTypeType.Other;

		public virtual IDataType GetTargetValueType(IPropertyDefinition propertyDefinition) => null;
	}
}