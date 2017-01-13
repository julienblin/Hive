using System.Threading;
using System.Threading.Tasks;
using Hive.Models;

namespace Hive.Handlers.Impl
{
	public class DefaultModelGetHandler<T> : IHandleGet<T>
		where T: class, IModel
	{
		public Task<IHandlerResult> Get(object id, CancellationToken ct)
		{
			throw new System.NotImplementedException();
		}
	}
}