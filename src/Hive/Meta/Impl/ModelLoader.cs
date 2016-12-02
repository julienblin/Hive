using System;
using System.Collections.Generic;
using System.Linq;
using Hive.Foundation.Entities;
using Hive.Foundation.Exceptions;
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
					Version = SemVer.Parse(modelData.Version),
					EntitiesBySingleName = modelData.Entities.Safe().ToDictionary(x => x.SingleName, MapEntityDefinition)
				};
				model.EntitiesByPluralName = model.EntitiesBySingleName.Values.ToDictionary(x => x.PluralName);
				model.FinishLoading(_valueTypeFactory);

				return model;

			}
			catch (Exception ex) when (!(ex is ModelLoadingException))
			{
				throw new ModelLoadingException($"There has been an error while loading the model {modelData.Name}", ex);
			}
		}

		private IEntityDefinition MapEntityDefinition(EntityDefinitionData entityDefinitionData)
		{
			return new EntityDefinition
			{
				OriginalData = entityDefinitionData,
				SingleName = entityDefinitionData.SingleName,
				PluralName = entityDefinitionData.PluralName,
				EntityType = entityDefinitionData.Type.ToEnum<EntityType>(),
				Properties = MapProperties(entityDefinitionData.Properties).ToDictionary(x => x.Name)
			};
		}

		private IEnumerable<IPropertyDefinition> MapProperties(IEnumerable<PropertyDefinitionData> properties)
		{
			return properties.Safe().Select(property => new PropertyDefinition
			{
				OriginalData = property,
				Name = property.Name,
				PropertyType = _valueTypeFactory.GetValueType(property.Type)
			});
		}
	}
}