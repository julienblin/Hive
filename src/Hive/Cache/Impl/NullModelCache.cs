using Hive.Meta;

namespace Hive.Cache.Impl
{
	public class NullModelCache : IModelCache
	{
		public IModel Get(string modelName)
		{
			return null;
		}

		public void Put(IModel model)
		{
		}

		public void Clear(string modelName)
		{
		}

		public void Clear()
		{
		}
	}
}