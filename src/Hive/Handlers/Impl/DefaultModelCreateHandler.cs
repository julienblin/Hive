using System.Threading;
using System.Threading.Tasks;
using Hive.Models;

namespace Hive.Handlers.Impl
{
	public class DefaultModelCreateHandler<T> : IHandleCreate<T>
		where T: class, IModel
	{
		public Task<IHandlerResult> Create(T resource, CancellationToken ct)
		{
			throw new System.NotImplementedException();
		}
	}
}