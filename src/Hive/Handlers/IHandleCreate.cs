using System.Threading;
using System.Threading.Tasks;

namespace Hive.Handlers
{
	public interface IHandleCreate<in T> : IHandler
	{
		Task<IHandlerResult> Create(T resource, CancellationToken ct);
	}
}