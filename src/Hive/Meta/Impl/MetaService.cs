using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hive.Cache;
using Hive.Config;
using Hive.Foundation.Entities;
using Hive.Foundation.Exceptions;
using Hive.Foundation.Extensions;
using Hive.Meta.Data;
using Hive.Meta.ValueTypes;

namespace Hive.Meta.Impl
{
	public class MetaService : IMetaService
	{
		private readonly IConfigService _configService;
		private readonly IMetaRepository _metaRepository;
		private readonly IValueTypeFactory _valueTypeFactory;
		private readonly IModelCache _cache;

		public MetaService(
			IConfigService configService,
			IMetaRepository metaRepository,
			IValueTypeFactory valueTypeFactory,
			IModelCache cache)
		{
			_configService = configService.NotNull(nameof(configService));
			_metaRepository = metaRepository.NotNull(nameof(metaRepository));
			_valueTypeFactory = valueTypeFactory.NotNull(nameof(valueTypeFactory));
			_cache = cache.NotNull(nameof(cache));
		}

		public async Task<IModel> GetModel(string modelName, CancellationToken ct)
		{
			var config = await _configService.GetConfig(ct);
			if (config.Mode == EnvironmentMode.Debug)
			{
				return await LoadModel(modelName, ct);
			}
			else
			{
				var cachedModel = _cache.Get(modelName);
				if (cachedModel != null) return cachedModel;

				var model = await LoadModel(modelName, ct);
				_cache.Put(model);
				return model;
			}
		}

		private async Task<IModel> LoadModel(string modelName, CancellationToken ct)
		{
			return CreateModelFromModelData(await _metaRepository.GetModel(modelName, ct));
		}

		private IModel CreateModelFromModelData(ModelData modelData)
		{
			try
			{
				var model = new Model
				{
					Name = modelData.Name,
					Version = SemVer.Parse(modelData.Version),
					EntitiesBySingleName = modelData.Entities.Safe().ToDictionary(x => x.SingleName, MapEntityDefinition)
				};
				model.EntitiesByPluralName = model.EntitiesBySingleName.Values.ToDictionary(x => x.PluralName);

				ResolveInternalReferences(model);

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
				SingleName = entityDefinitionData.SingleName,
				PluralName = entityDefinitionData.PluralName,
				EntityType = entityDefinitionData.Type.ToEnum<EntityType>(),
				Properties = MapProperties(entityDefinitionData.Properties).ToDictionary(x => x.Name)
			};
		}

		private IEnumerable<IPropertyDefinition> MapProperties(PropertyDefinitionData[] properties)
		{
			return properties.Safe().Select(property => new PropertyDefinition
			{
				Name = property.Name,
				PropertyType = _valueTypeFactory.GetValueType(property.Type),
				PropertyDefinitionData = property
			});
		}

		private void ResolveInternalReferences(Model model)
		{
			foreach (EntityDefinition entityDefinition in model.EntitiesBySingleName.Values)
			{
				entityDefinition.ResolveReferences(_valueTypeFactory, model);
			}
		}
	}
}