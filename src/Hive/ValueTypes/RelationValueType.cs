using Hive.Entities;
using Hive.Exceptions;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.ValueTypes
{
	public class RelationValueType : ValueType<IEntity>
	{
		private const string PropertyTarget = "target";
		private const string PropertyInverseOf = "inverseOf";

		public RelationValueType()
			: base("relation")
		{
		}

		public override object ConvertFromPropertyBagValue(IPropertyDefinition propertyDefinition, object value)
		{
			var bag = value as PropertyBag;
			if (bag == null) return null;

			if (propertyDefinition.AdditionalProperties.SafeGet(PropertyInverseOf) != null)
				return null;

			var entityId = bag[MetaConstants.IdProperty];
			if (entityId == null) return null;

			var entityDefinition = (IEntityDefinition)propertyDefinition.AdditionalProperties[PropertyTarget];

			var entity = propertyDefinition.EntityDefinition.Model.Factories.Entity.Hydrate(entityDefinition, bag);

			return entity;
		}

		public override object ConvertToPropertyBagValue(IPropertyDefinition propertyDefinition, object value, bool keepRelationInfo)
		{
			var entity = value as IEntity;
			if (entity == null) return null;

			if (!keepRelationInfo && (propertyDefinition.AdditionalProperties.SafeGet(PropertyInverseOf) != null))
				return null;

			if(keepRelationInfo)
				return entity.ToPropertyBag(true);
			
			return new PropertyBag
			{
				[MetaConstants.IdProperty] = entity.Id
			};
		}

		public override void ModelLoaded(IPropertyDefinition propertyDefinition)
		{
			var target = propertyDefinition.PropertyBag[PropertyTarget] as string;
			if(target.IsNullOrEmpty())
				throw new ModelLoadingException($"When creating a relation, the {PropertyTarget} property must be specified (for {propertyDefinition}).");

			var entityDefinition = propertyDefinition.EntityDefinition.Model.EntitiesBySingleName.SafeGet(target);
			if(entityDefinition == null)
				throw new ModelLoadingException($"Unable to find the target entity {target} for {propertyDefinition}.");

			if(entityDefinition.EntityType == EntityType.None)
				throw new ModelLoadingException($"Unable to create a relation to the target entity {entityDefinition} because its entity type is {nameof(EntityType.None)}. (for {propertyDefinition}).");

			propertyDefinition.AdditionalProperties[PropertyTarget] = entityDefinition;

			var inverseOf = propertyDefinition.PropertyBag[PropertyInverseOf] as string;
			if (!inverseOf.IsNullOrEmpty())
			{
				var inverseOfPropertyDefinition = entityDefinition.Properties.SafeGet(inverseOf);
				if (inverseOfPropertyDefinition == null)
					throw new ModelLoadingException($"Did not find inverse relation with name {inverseOf} for {propertyDefinition}");

				propertyDefinition.AdditionalProperties[PropertyInverseOf] = inverseOfPropertyDefinition;
			}
		}

		public override DataTypeType DataTypeType => DataTypeType.Relation;

		public override IDataType GetTargetValueType(IPropertyDefinition propertyDefinition) => (IEntityDefinition)propertyDefinition.AdditionalProperties[PropertyTarget];
	}
}