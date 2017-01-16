using System.Threading;
using System.Threading.Tasks;

namespace Hive.Handlers
{
	public interface IHandleUpdate<in TResource>
	{
		Task<IHandlerResult> Update(TResource resource, CancellationToken ct);
	}
}