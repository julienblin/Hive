using Hive.Meta;
using Hive.Meta.Data;

namespace Hive.Tests.Mocks
{
	public class ModelLoaderMock : IModelLoader
	{
		private readonly IModel _modelToReturn;

		public ModelLoaderMock()
		{
		}

		public ModelLoaderMock(IModel modelToReturn)
		{
			_modelToReturn = modelToReturn;
		}

		public IModel Load(ModelData modelData)
		{
			return _modelToReturn;
		}
	}
}