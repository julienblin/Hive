﻿using System.Collections.Generic;
using Hive.Foundation.Exceptions;
using Hive.Foundation.Extensions;
using Hive.Meta.Data;

namespace Hive.Meta.Impl
{
	internal class PropertyDefinition : IPropertyDefinition, IOriginalDataHolder<PropertyDefinitionData>
	{
		private IDictionary<string, object> _properties = new Dictionary<string, object>();

		public string Name { get; set; }

		public IDataType PropertyType { get; set; }

		public T GetProperty<T>(string propertyName)
		{
			var result = _properties.SafeGet(propertyName);
			return result == null ? default(T) : (T)result;
		}

		public void SetProperty(string propertyName, object value)
		{
			_properties[propertyName] = value;
		}

		public PropertyDefinitionData OriginalData { get; set; }

		internal void FinishLoading(IValueTypeFactory valueTypeFactory, Model model, EntityDefinition entityDefinition)
		{
			if (PropertyType == null)
			{
				PropertyType = model.EntitiesBySingleName.SafeGet(OriginalData.Type);
			}

			if (PropertyType == null)
			{
				throw new ModelLoadingException($"Unable to resolve property type {OriginalData.Type} for {model}.{entityDefinition}.{this}.");
			}

			if (PropertyType is IValueType)
			{
				((IValueType) PropertyType).FinishLoading(valueTypeFactory, model, entityDefinition, this);
			}
		}

		public override string ToString() => Name;
	}
}