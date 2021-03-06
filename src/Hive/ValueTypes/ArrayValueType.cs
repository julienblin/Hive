﻿using System;
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

		public override object ConvertToPropertyBagValue(IPropertyDefinition propertyDefinition, object value, bool keepRelationInfo)
		{
			var netvalue = value as List<object>;
			if (netvalue == null || netvalue.Count == 0) return null;

			var itemsPropertyDefinition = (IPropertyDefinition)propertyDefinition.AdditionalProperties[PropertyItems];
			var targetList = netvalue
				.Select(x => itemsPropertyDefinition.PropertyType.ConvertToPropertyBagValue(itemsPropertyDefinition, x, keepRelationInfo))
				.Where(x => x != null)
				.ToArray();
			if (!targetList.Any()) return null;
			var targetListItemsTypes = targetList.First().GetType();
			var result = Array.CreateInstance(targetListItemsTypes, targetList.Length);
			Array.Copy(targetList, result, targetList.Length);
			return result;
		}

		public override void ModelLoaded(IPropertyDefinition propertyDefinition)
		{
			var untypedItemsData = propertyDefinition.PropertyBag[PropertyItems];

			if (untypedItemsData is PropertyBag)
			{
				var propertyBagItemsData = (PropertyBag) untypedItemsData;
				if ((propertyBagItemsData["type"] as string).IsNullOrEmpty())
					throw new ModelLoadingException($"When using inline property definitions, the property type is mandatory (on {propertyDefinition}).");

				var itemsType = propertyDefinition.GetValueTypeOrEntityDefinition(propertyBagItemsData["type"] as string);
				if (itemsType == null)
					throw new ModelLoadingException(
						$"Unable to find an entity or a value type named {propertyBagItemsData["type"]} (on {propertyDefinition}).");

				propertyBagItemsData["name"] = propertyDefinition.Name + "_" + PropertyItems;
				propertyBagItemsData["description"] = $"Items for {propertyDefinition}";
				var itemsPropertyDefinition = PropertyDefinitionFactory.Create(propertyDefinition.EntityDefinition, itemsType, propertyBagItemsData);
				itemsPropertyDefinition.PropertyType.ModelLoaded(itemsPropertyDefinition);
				propertyDefinition.AdditionalProperties[PropertyItems] = itemsPropertyDefinition;

				return;
			}

			if (untypedItemsData is string)
			{
				var itemsType = propertyDefinition.GetValueTypeOrEntityDefinition((string) untypedItemsData);
				if (itemsType == null)
					throw new ModelLoadingException(
						$"Unable to find an entity or a value type named {untypedItemsData} (on {propertyDefinition}).");
				var itemsPropertyBag = new PropertyBag
				{
					["name"] = propertyDefinition.Name + "_" + PropertyItems,
					["description"] = $"Items for {propertyDefinition}"
				};

				var itemsPropertyDefinition = PropertyDefinitionFactory.Create(propertyDefinition.EntityDefinition, itemsType, itemsPropertyBag);
				itemsPropertyDefinition.PropertyType.ModelLoaded(itemsPropertyDefinition);
				propertyDefinition.AdditionalProperties[PropertyItems] = itemsPropertyDefinition;

				return;
			}

			throw new ModelLoadingException(
					$"An array must have an item property that points to either a value type or an entity (on {propertyDefinition}).");
		}

		public override DataTypeType DataTypeType => DataTypeType.Container;

		public override IDataType GetTargetValueType(IPropertyDefinition propertyDefinition) => ((IPropertyDefinition)propertyDefinition.AdditionalProperties[PropertyItems]).PropertyType;
	}
}