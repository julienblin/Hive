using System.Threading;
using System.Threading.Tasks;

namespace Hive.Handlers
{
	public interface IHandleCreate<in TResource>
		where TResource : class
	{
		Task<IHandlerResult> Create(TResource resource, CancellationToken ct);
	}
}