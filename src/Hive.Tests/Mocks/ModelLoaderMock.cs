using Hive.Foundation.Entities;
using Hive.Meta;

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

		public IModel Load(PropertyBag modelData)
		{
			return _modelToReturn;
		}
	}
}