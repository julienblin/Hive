using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hive.Cache;
using Hive.Config;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta.Data;

namespace Hive.Meta.Impl
{
	public class MetaService : IMetaService
	{
		private readonly IMetaRepository _metaRepository;
		private readonly IModelLoader _modelLoader;
		private readonly ICache<IModel> _cache;

		public MetaService(
			IMetaRepository metaRepository,
			IModelLoader modelLoader,
			ICache<IModel> cache)
		{
			_metaRepository = metaRepository.NotNull(nameof(metaRepository));
			_modelLoader = modelLoader.NotNull(nameof(modelLoader));
			_cache = cache.NotNull(nameof(cache));
		}

		public async Task<IModel> GetModel(string modelName, CancellationToken ct)
		{
			var cachedModel = _cache.Get(modelName);
			if (cachedModel != null) return cachedModel;

			var model = _modelLoader.Load(await _metaRepository.GetModel(modelName, ct));
			_cache.Put(model.Name, model);
			return model;
		}
	}
}