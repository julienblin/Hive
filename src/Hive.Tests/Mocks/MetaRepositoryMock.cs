using System.Threading;
using System.Threading.Tasks;
using Hive.Foundation.Entities;
using Hive.Meta;

namespace Hive.Tests.Mocks
{
	public class MetaRepositoryMock : IMetaRepository
	{
		private readonly PropertyBag _modelData;

		public MetaRepositoryMock()
		{
		}

		public MetaRepositoryMock(PropertyBag modelData)
		{
			_modelData = modelData;
		}

		public Task<PropertyBag> GetModel(string modelName, CancellationToken ct)
		{
			return Task.FromResult(_modelData);
		}
	}
}