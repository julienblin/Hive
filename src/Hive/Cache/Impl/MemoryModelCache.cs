using System;
using Hive.Meta;
using Microsoft.Extensions.Caching.Memory;

namespace Hive.Cache.Impl
{
	public class MemoryModelCache : IModelCache
	{
		private readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions { ExpirationScanFrequency = TimeSpan.MaxValue });

		public IModel Get(string modelName)
		{
			return _cache.Get<IModel>(modelName);
		}

		public void Put(IModel model)
		{
			_cache.Set(model.Name, model);
		}

		public void Clear(string modelName)
		{
			_cache.Remove(modelName);
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}
	}
}