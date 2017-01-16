using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;

namespace Hive.Handlers
{
	public interface IHandleDelete<in TResource, in TId>
		where TResource : IIdentifiable<TId>
	{
		Task<IHandlerResult> Delete(TId id, CancellationToken ct);
	}
}