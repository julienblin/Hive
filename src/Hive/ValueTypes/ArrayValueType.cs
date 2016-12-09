using System;
using System.Collections.Generic;
using Hive.Exceptions;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.Meta.Impl;
using System.Linq;

namespace Hive.ValueTypes
{
	public class ArrayValueType : ValueType<List<object>>
	{
		private const string PropertyItems = "items";

		public ArrayValueType()
			: base("array")
		{
		}

		public override object ConvertFromPropertyBagValue(IPropertyDefinition propertyDefinition, object value)
		{
			var arrayValue = value as Array;
			if (arrayValue == null) return null;

			var itemsPropertyDefinition = (IPropertyDefinition)propertyDefinition.AdditionalProperties[PropertyItems];

			var result = new List<object>(arrayValue.Length);

			foreach (var itemValue in arrayValue)
			{
				var resultItem = itemsPropertyDefinition.PropertyType.ConvertFromPropertyBagValue(itemsPropertyDefinition, itemValue);
				result.Add(resultItem);
			}

			return result;
		}

		public override object ConvertToPropertyBagValue(IPropertyDefinition propertyDefinition, object value)
		{
			var netvalue = value as List<object>;
			if (netvalue == null || netvalue.Count == 0) return null;

			var itemsPropertyDefinition = (IPropertyDefinition)propertyDefinition.AdditionalProperties[PropertyItems];
			var targetList = netvalue
				.Select(x => itemsPropertyDefinition.PropertyType.ConvertToPropertyBagValue(itemsPropertyDefinition, x))
				.ToArray();
			if (targetList.Length == 0) return null;
			var targetListItemsTypes = targetList.First().GetType();
			var result = Array.CreateInstance(targetListItemsTypes, targetList.Length);
			Array.Copy(targetList, result, targetList.Length);
			return result;
		}

		public override void FinishLoading(IValueTypeFactory valueTypeFactory, IPropertyDefinition propertyDefinition)
		{
			var untypedItemsData = propertyDefinition.PropertyBag["items"];

			if (untypedItemsData is PropertyBag)
			{
				var propertyBagItemsData = (PropertyBag) untypedItemsData;
				if ((propertyBagItemsData["type"] as string).IsNullOrEmpty())
					throw new ModelLoadingException($"When using inline property definitions, the property type is mandatory (on {propertyDefinition}).");

				var itemsType = (IDataType)valueTypeFactory.GetValueType(propertyBagItemsData["type"] as string) ??
							propertyDefinition.EntityDefinition.Model.EntitiesBySingleName.SafeGet(propertyBagItemsData["type"] as string);
				if (itemsType == null)
					throw new ModelLoadingException(
						$"Unable to find an entity or a value type named {propertyBagItemsData["type"]} (on {propertyDefinition}).");

				propertyDefinition.AdditionalProperties[PropertyItems] = new PropertyDefinition
				{
					EntityDefinition = propertyDefinition.EntityDefinition,
					PropertyBag = propertyBagItemsData,
					Name = propertyDefinition.Name + "_items",
					PropertyType = itemsType,
					DefaultValue = propertyBagItemsData["default"]
				};

				return;
			}

			if (untypedItemsData is string)
			{
				var strItemsData = (string) untypedItemsData;

				var itemsType = (IDataType)valueTypeFactory.GetValueType(strItemsData) ??
							propertyDefinition.EntityDefinition.Model.EntitiesBySingleName.SafeGet(strItemsData);
				if (itemsType == null)
					throw new ModelLoadingException(
						$"Unable to find an entity or a value type named {strItemsData} (on {propertyDefinition}).");

				propertyDefinition.AdditionalProperties[PropertyItems] = new PropertyDefinition
				{
					EntityDefinition = propertyDefinition.EntityDefinition,
					PropertyBag = new PropertyBag(),
					Name = propertyDefinition.Name + "_items",
					PropertyType = itemsType,
					DefaultValue = null
				};

				return;
			}

			throw new ModelLoadingException(
					$"An array must have an item property that points to either a value type or an entity (on {propertyDefinition}).");
		}
	}
}