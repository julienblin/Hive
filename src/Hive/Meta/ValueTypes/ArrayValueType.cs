using System.Collections.Generic;
using Hive.Foundation.Exceptions;
using Hive.Foundation.Extensions;
using Hive.Meta.Data;
using Hive.Meta.Impl;

namespace Hive.Meta.ValueTypes
{
	public class ArrayValueType : ValueType
	{
		private const string PropertyItems = "items";

		public ArrayValueType()
			: base("array")
		{
		}

		public override IDictionary<string, object> LoadAdditionalProperties(IValueTypeFactory valueTypeFactory, IModel model, PropertyDefinitionData propertyDefinitionData)
		{
			var result = new Dictionary<string, object>();

			var itemsData = propertyDefinitionData.GetValue<string>("items");
			if (itemsData == null)
			{
				throw new ModelLoadingException($"An array must have an item property that points to either a value type or an entity (for property {propertyDefinitionData.Name}).");
			}

			var itemsType = (IDataType) valueTypeFactory.GetValueType(itemsData) ?? model.EntitiesBySingleName.SafeGet(itemsData);
			if (itemsType == null)
			{
				throw new ModelLoadingException($"Unable to find an entity or a value type named {itemsData} (for property {propertyDefinitionData.Name}).");
			}

			result[PropertyItems] = itemsType;

			return result;
		}
	}
}