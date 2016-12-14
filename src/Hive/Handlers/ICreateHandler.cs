using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;

namespace Hive.Handlers
{
	public interface ICreateHandler
	{
		Task<IEntity> Create(IEntity entity, CancellationToken ct);
	}
}