using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;

namespace Hive.Handlers
{
	public interface IHandleGet<TResource, in TId>
		where TResource : IIdentifiable<TId>
	{
		Task<IHandlerResult> Get(TId id, CancellationToken ct);
	}
}