using System.Collections.Generic;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Entities
{
	public class Entity : IEntity
	{
		private readonly IDictionary<string, object> _propertyValues = new Dictionary<string, object>();

		public Entity(IEntityDefinition definition)
		{
			Definition = definition.NotNull(nameof(definition));
		}

		public IEntityDefinition Definition { get; }

		public object Id
		{
			get { return GetPropertyValue<object>(MetaConstants.IdProperty); }
			set { SetPropertyValue(MetaConstants.IdProperty, value); }
		}

		public void SetPropertyValue(string propertyName, object value)
		{
			var propertyDefinition = Definition.Properties.SafeGet(propertyName);
			if (propertyDefinition != null)
			{
				_propertyValues[propertyName] = propertyDefinition.PropertyType.ConvertValue(propertyDefinition, value);
			}
		}

		public T GetPropertyValue<T>(string propertyName)
		{
			var value = _propertyValues.SafeGet(propertyName);
			return value != null ? (T) value : default(T);
		}

		public bool HasPropertyValue(string propertyName)
		{
			var value = _propertyValues.SafeGet(propertyName);
			return value != null;
		}

		public override string ToString() => $"{Definition} ({Id})";
	}
}