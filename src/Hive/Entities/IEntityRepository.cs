using System.Threading;
using System.Threading.Tasks;
using Hive.Meta;
using Hive.Queries;

namespace Hive.Entities
{
	public interface IEntityRepository
	{
		IQuery CreateQuery(IEntityDefinition entityDefinition);

		Task<IEntity> Create(IEntity entity, CancellationToken ct);

		Task<IEntity> Update(IEntity entity, CancellationToken ct);

		Task<bool> Delete(IEntityDefinition entityDefinition, object id, CancellationToken ct);
	}
}