using System.Threading;
using System.Threading.Tasks;
using Hive.Cache;
using Hive.Foundation.Extensions;

namespace Hive.Meta.Impl
{
	public class MetaService : IMetaService
	{
		private readonly IModelCache _cache;
		private readonly IMetaRepository _metaRepository;
		private readonly IModelLoader _modelLoader;

		public MetaService(
			IMetaRepository metaRepository,
			IModelLoader modelLoader,
			IModelCache cache)
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
			_cache.Put(model);
			return model;
		}
	}
}