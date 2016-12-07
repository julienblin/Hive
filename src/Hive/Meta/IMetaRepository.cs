using System.Threading;
using System.Threading.Tasks;
using Hive.Foundation.Entities;

namespace Hive.Meta
{
	public interface IMetaRepository
	{
		Task<PropertyBag> GetModel(string modelName, CancellationToken ct);
	}
}