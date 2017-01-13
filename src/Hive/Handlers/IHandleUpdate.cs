using System.Threading;
using System.Threading.Tasks;

namespace Hive.Handlers
{
	public interface IHandleUpdate<in T> : IHandler
	{
		Task<IHandlerResult> Update(T resource, CancellationToken ct);
	}
}