using System;
using System.Collections.Generic;
using Hive.Exceptions;
using Hive.Foundation.Extensions;
using Hive.Meta.Data;
using Hive.Meta.Impl;

namespace Hive.Meta.ValueTypes
{
	public class ArrayValueType : ValueType<Array>
	{
		private const string PropertyItems = "items";

		public ArrayValueType()
			: base("array")
		{
		}

		public override void FinishLoading(IValueTypeFactory valueTypeFactory, IPropertyDefinition propertyDefinition)
		{
			base.FinishLoading(valueTypeFactory, propertyDefinition);

			var propertyOriginalData = propertyDefinition as IOriginalDataHolder<PropertyDefinitionData>;
			if (propertyOriginalData == null)
			{
				throw new ModelLoadingException($"Unable to finish the loading of property {propertyDefinition} because it does not implement {nameof(IOriginalDataHolder<PropertyDefinitionData>)}.");
			}

			var itemsData = propertyOriginalData.OriginalData.GetValue<string>("items");
			if (itemsData == null)
			{
				throw new ModelLoadingException($"An array must have an item property that points to either a value type or an entity (on {propertyDefinition}).");
			}

			var itemsType = (IDataType)valueTypeFactory.GetValueType(itemsData) ?? propertyDefinition.EntityDefinition.Model.EntitiesBySingleName.SafeGet(itemsData);
			if (itemsType == null)
			{
				throw new ModelLoadingException($"Unable to find an entity or a value type named {itemsData} (on {propertyDefinition}).");
			}

			propertyDefinition.SetProperty(PropertyItems, itemsType);
		}
	}
}