using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;

namespace Hive.Handlers.Impl
{
	public class DefaultModelGetHandler<T> : IHandleGet<T>
		where T: class, IEntity
	{
		public Task<IHandlerResult> Get(object id, CancellationToken ct)
		{
			throw new System.NotImplementedException();
		}
	}
}