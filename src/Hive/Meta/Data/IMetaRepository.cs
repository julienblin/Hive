using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Hive.Meta.Data
{
	public interface IMetaRepository
	{
		Task<ModelData> GetModel(string modelName, CancellationToken ct);
	}
}