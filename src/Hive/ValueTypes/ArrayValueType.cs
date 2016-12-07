using System;
using Hive.Exceptions;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.ValueTypes
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
			var itemsData = propertyDefinition.PropertyBag["items"] as string;
			if (itemsData == null)
				throw new ModelLoadingException(
					$"An array must have an item property that points to either a value type or an entity (on {propertyDefinition}).");

			var itemsType = (IDataType) valueTypeFactory.GetValueType(itemsData) ??
			                propertyDefinition.EntityDefinition.Model.EntitiesBySingleName.SafeGet(itemsData);
			if (itemsType == null)
				throw new ModelLoadingException(
					$"Unable to find an entity or a value type named {itemsData} (on {propertyDefinition}).");

			propertyDefinition.AdditionalProperties[PropertyItems] = itemsType;
		}
	}
}