using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Handlers.Impl
{
	public class DeleteHandler : IDeleteHandler
	{
		private readonly IEntityRepository _entityRepository;

		public DeleteHandler(IEntityRepository entityRepository)
		{
			_entityRepository = entityRepository.NotNull(nameof(entityRepository));
		}

		public Task<bool> Delete(IEntityDefinition entityDefinition, object id, CancellationToken ct)
		{
			entityDefinition.NotNull(nameof(entityDefinition));
			id.NotNull(nameof(id));

			return _entityRepository.Delete(entityDefinition, id, ct);
		}
	}
}