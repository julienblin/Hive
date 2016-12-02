using System.Collections.Generic;
using Hive.Foundation.Extensions;
using Hive.Meta.Data;
using Hive.Meta.Impl;

namespace Hive.Meta.ValueTypes
{
	public abstract class ValueType : IValueType
	{
		protected ValueType(string name)
		{
			Name = name.NotNullOrEmpty(nameof(name));
		}

		public string Name { get; }

		public virtual void FinishLoading(IValueTypeFactory valueTypeFactory, IPropertyDefinition propertyDefinition)
		{	
		}
	}
}