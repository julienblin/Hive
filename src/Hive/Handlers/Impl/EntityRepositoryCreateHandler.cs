using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;

namespace Hive.Handlers.Impl
{
	public class EntityRepositoryCreateHandler<T> : IHandleCreate<T>
		where T: class, IEntity
	{
		public Task<IHandlerResult> Create(T resource, CancellationToken ct)
		{
			throw new System.NotImplementedException();
		}
	}
}