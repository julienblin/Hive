using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Meta;

namespace Hive.Handlers
{
	public interface IDeleteHandler
	{
		Task<bool> Delete(IEntityDefinition entityDefinition, object id, CancellationToken ct);
	}
}