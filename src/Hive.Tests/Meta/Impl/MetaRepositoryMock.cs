using System.Threading;
using System.Threading.Tasks;
using Hive.Meta.Data;

namespace Hive.Tests.Meta.Impl
{
	public class MetaRepositoryMock : IMetaRepository
	{
		private readonly ModelData _modelData;

		public MetaRepositoryMock(ModelData modelData)
		{
			_modelData = modelData;
		}

		public Task<ModelData> GetModel(string modelName, CancellationToken ct)
		{
			return Task.FromResult(_modelData);
		}
	}
}