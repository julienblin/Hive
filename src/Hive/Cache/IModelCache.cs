using Hive.Meta;

namespace Hive.Cache
{
	public interface IModelCache
	{
		IModel Get(string modelName);

		void Put(IModel model);

		void Clear(string modelName);

		void Clear();
	}
}