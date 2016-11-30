using System.Threading;
using System.Threading.Tasks;

namespace Hive.Meta
{
	public interface IMetaService
	{
		Task<IModel> GetModel(string modelName, CancellationToken ct);
	}
}