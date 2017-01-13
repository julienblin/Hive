using System.Threading;
using System.Threading.Tasks;
using Hive.Models;

namespace Hive.Handlers.Impl
{
	public class DefaultModelUpdateHandler<T> : IHandleUpdate<T>
		where T: class, IModel
	{
		public Task<IHandlerResult> Update(T resource, CancellationToken ct)
		{
			throw new System.NotImplementedException();
		}
	}
}