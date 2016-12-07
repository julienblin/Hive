using System;
using System.Collections.Generic;
using System.Linq;
using Hive.Exceptions;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.ValueTypes;

namespace Hive.Meta.Impl
{
	public class ModelLoader : IModelLoader
	{
		private readonly IValueTypeFactory _valueTypeFactory;

		public ModelLoader(IValueTypeFactory valueTypeFactory)
		{
			_valueTypeFactory = valueTypeFactory.NotNull(nameof(valueTypeFactory));
		}

		public IModel Load(PropertyBag modelData)
		{
			try
			{
				var model = new Model
				{
					PropertyBag = modelData,
					Name = modelData["name"] as string,
					Version = SemVer.Parse(modelData["version"] as string)
				};
				model.EntitiesBySingleName = (modelData["entities"] as PropertyBag[])
					.Safe()
					.ToDictionary(x => x["singlename"] as string, x => MapEntityDefinition(model, x));
				model.EntitiesByPluralName = model.EntitiesBySingleName.Values.ToDictionary(x => x.PluralName);

				model.FinishLoading(_valueTypeFactory);

				return model;

			}
			catch (Exception ex) when (!(ex is ModelLoadingException))
			{
				throw new ModelLoadingException($"There has been an error while loading the model {modelData["name"]}", ex);
			}
		}

		private IEntityDefinition MapEntityDefinition(IModel model, PropertyBag entityDefinitionData)
		{
			var entityDefinition = new EntityDefinition
			{
				Model = model,
				PropertyBag = entityDefinitionData,
				SingleName = entityDefinitionData["singlename"] as string,
				PluralName = entityDefinitionData["pluralname"] as string,
				EntityType = (entityDefinitionData["type"] as string).ToEnum<EntityType>()
			};
			entityDefinition.Properties =
				MapProperties(entityDefinition, entityDefinitionData["properties"] as PropertyBag[]).ToDictionary(x => x.Name);

			return entityDefinition;
		}

		private IEnumerable<IPropertyDefinition> MapProperties(IEntityDefinition entityDefinition, PropertyBag[] properties)
		{
			var propertyDefinitions = properties.Safe().Select(property => new PropertyDefinition
			{
				EntityDefinition = entityDefinition,
				PropertyBag = property,
				Name = property["name"] as string,
				PropertyType = _valueTypeFactory.GetValueType(property["type"] as string),
				DefaultValue = property["default"]
			}).ToList();

			if ((entityDefinition.EntityType != EntityType.None) && !propertyDefinitions.Any(x => x.Name.SafeOrdinalEquals(MetaConstants.IdProperty)))
			{
				
				propertyDefinitions.Add(new DefaultIdPropertyDefinition(entityDefinition, _valueTypeFactory.GetValueType("uuid")));
			}

			return propertyDefinitions;
		}
	}
}