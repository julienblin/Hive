using Hive.Cache;
using Hive.Meta;

namespace Hive.Tests.Meta.Impl
{
	public class ModelCacheMock : IModelCache
	{
		private readonly IModel _returnedModel;

		public ModelCacheMock(IModel returnedModel = null)
		{
			_returnedModel = returnedModel;
		}

		public IModel Get(string modelName)
		{
			return _returnedModel;
		}

		public void Put(IModel model)
		{
			//
		}
	}
}