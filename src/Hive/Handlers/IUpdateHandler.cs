using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;

namespace Hive.Handlers
{
	public interface IUpdateHandler
	{
		Task<IEntity> Update(IEntity entity, CancellationToken ct);
	}
}