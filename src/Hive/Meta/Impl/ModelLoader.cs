using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Hive.Entities;
using Hive.Exceptions;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Handlers.Impl;
using Hive.Validation;
using Hive.ValueTypes;

namespace Hive.Meta.Impl
{
	public class ModelLoader : IModelLoader
	{
		private readonly IValueTypeFactory _valueTypeFactory;
		private readonly IValidatorFactory _validatorFactory;
		private readonly IEntityFactory _entityFactory;
		private readonly IEntityRepository _entityRepository;

		public ModelLoader(
			IValueTypeFactory valueTypeFactory,
			IValidatorFactory validatorFactory,
			IEntityFactory entityFactory,
			IEntityRepository entityRepository)
		{
			_valueTypeFactory = valueTypeFactory.NotNull(nameof(valueTypeFactory));
			_validatorFactory = validatorFactory.NotNull(nameof(validatorFactory));
			_entityFactory = entityFactory.NotNull(nameof(entityFactory));
			_entityRepository = entityRepository.NotNull(nameof(entityRepository));
		}

		public IModel Load(PropertyBag modelData)
		{
			try
			{
				var model = new Model(
					modelData["name"] as string,
					SemVer.Parse(modelData["version"] as string),
					new ModelFactories(_valueTypeFactory, _validatorFactory, _entityFactory))
				{
					PropertyBag = modelData
				};
				model.EntitiesBySingleName = (modelData["entities"] as PropertyBag[])
					.Safe()
					.ToImmutableDictionary(x => x["singlename"] as string, x => MapEntityDefinition(model, x),
						StringComparer.OrdinalIgnoreCase);
				model.EntitiesByPluralName = model.EntitiesBySingleName.Values.ToImmutableDictionary(x => x.PluralName);

				model.ModelLoaded();

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
				EntityType = (entityDefinitionData["type"] as string).ToEnum<EntityType>(),
				ConcurrencyHandling = (entityDefinitionData["concurrency"] as string).ToEnum<ConcurrencyHandling>(),
				CreateHandler = new CreateHandler(_entityRepository),
				UpdateHandler = new UpdateHandler(_entityRepository),
				DeleteHandler = new DeleteHandler(_entityRepository)
			};
			entityDefinition.Properties =
				MapProperties(entityDefinition, entityDefinitionData["properties"] as PropertyBag[])
					.ToImmutableDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

			return entityDefinition;
		}

		private IEnumerable<IPropertyDefinition> MapProperties(IEntityDefinition entityDefinition, PropertyBag[] properties)
		{
			var propertyDefinitions = properties
				.Safe()
				.Select(property =>
				PropertyDefinitionFactory.Create(entityDefinition, _valueTypeFactory.GetValueType(property["type"] as string), property))
				.ToList();

			if ((entityDefinition.EntityType != EntityType.None) &&
			    !propertyDefinitions.Any(x => x.Name.SafeOrdinalEquals(MetaConstants.IdProperty)))
				propertyDefinitions.Add(new DefaultIdPropertyDefinition(entityDefinition, _valueTypeFactory.GetValueType("uuid")));

			return propertyDefinitions;
		}
	}
}