using System.Threading;
using System.Threading.Tasks;

namespace Hive.Handlers
{
	public interface IHandleDelete<in T> : IHandler
	{
		Task<IHandlerResult> Delete(T resource, CancellationToken ct);
	}
}