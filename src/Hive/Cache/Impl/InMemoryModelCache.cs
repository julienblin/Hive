using System.Collections.Concurrent;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Cache.Impl
{
	public class InMemoryModelCache : IModelCache
	{
		private readonly ConcurrentDictionary<string, IModel> _models = new ConcurrentDictionary<string, IModel>();

		public IModel Get(string modelName)
		{
			modelName.NotNullOrEmpty(nameof(modelName));
			IModel value;
			return _models.TryGetValue(modelName, out value) ? value : null;
		}

		public void Put(IModel model)
		{
			model.NotNull(nameof(model));

			_models.AddOrUpdate(model.Name, model, (modelName, updatedModel) => model);
		}
	}
}