using System.Threading;
using System.Threading.Tasks;

namespace Hive.Foundation.Lifecycle
{
	public interface IStartable
	{
		Task Start(CancellationToken ct);
	}
}