using System;
using System.Collections.Generic;
using System.Linq;
using Hive.Exceptions;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta.Data;

namespace Hive.Meta.Impl
{
	public class ModelLoader : IModelLoader
	{
		private readonly IValueTypeFactory _valueTypeFactory;

		public ModelLoader(IValueTypeFactory valueTypeFactory)
		{
			_valueTypeFactory = valueTypeFactory.NotNull(nameof(valueTypeFactory));
		}

		public IModel Load(ModelData modelData)
		{
			try
			{
				var model = new Model
				{
					OriginalData = modelData,
					Name = modelData.Name,
					Version = SemVer.Parse(modelData.Version)
				};
				model.EntitiesBySingleName = modelData.Entities.Safe()
					.ToDictionary(x => x.SingleName, x => MapEntityDefinition(model, x));
				model.EntitiesByPluralName = model.EntitiesBySingleName.Values.ToDictionary(x => x.PluralName);

				model.FinishLoading(_valueTypeFactory);

				return model;

			}
			catch (Exception ex) when (!(ex is ModelLoadingException))
			{
				throw new ModelLoadingException($"There has been an error while loading the model {modelData.Name}", ex);
			}
		}

		private IEntityDefinition MapEntityDefinition(IModel model, EntityDefinitionData entityDefinitionData)
		{
			var entityDefinition = new EntityDefinition
			{
				Model = model,
				OriginalData = entityDefinitionData,
				SingleName = entityDefinitionData.SingleName,
				PluralName = entityDefinitionData.PluralName,
				EntityType = entityDefinitionData.Type.ToEnum<EntityType>()
			};
			entityDefinition.Properties =
				MapProperties(entityDefinition, entityDefinitionData.Properties).ToDictionary(x => x.Name);

			return entityDefinition;
		}

		private IEnumerable<IPropertyDefinition> MapProperties(IEntityDefinition entityDefinition, IEnumerable<PropertyDefinitionData> properties)
		{
			var propertyDefinitions = properties.Safe().Select(property => new PropertyDefinition
			{
				EntityDefinition = entityDefinition,
				OriginalData = property,
				Name = property.Name,
				PropertyType = _valueTypeFactory.GetValueType(property.Type)
			}).ToList();

			if (!propertyDefinitions.Any(x => x.Name.SafeOrdinalEquals(MetaConstants.IdProperty)))
			{
				propertyDefinitions.Add(new PropertyDefinition
				{
					Name = MetaConstants.IdProperty,
					EntityDefinition = entityDefinition,
					PropertyType = _valueTypeFactory.GetValueType("uuid")
				});
			}

			return propertyDefinitions;
		}
	}
}