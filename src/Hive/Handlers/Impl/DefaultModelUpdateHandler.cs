using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;

namespace Hive.Handlers.Impl
{
	public class DefaultModelUpdateHandler<T> : IHandleUpdate<T>
		where T: class, IEntity
	{
		public Task<IHandlerResult> Update(T resource, CancellationToken ct)
		{
			throw new System.NotImplementedException();
		}
	}
}