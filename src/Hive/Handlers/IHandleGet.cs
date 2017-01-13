using System.Threading;
using System.Threading.Tasks;

namespace Hive.Handlers
{
	public interface IHandleGet<in T> : IHandler
	{
		Task<IHandlerResult> Get(object id, CancellationToken ct);
	}
}