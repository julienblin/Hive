using System.Threading;
using System.Threading.Tasks;

namespace Hive.Config
{
	public interface IConfigService
    {
		Task<IHiveConfig> GetConfig(CancellationToken ct);
    }
}
