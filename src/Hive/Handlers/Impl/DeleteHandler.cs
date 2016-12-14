using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Handlers.Impl
{
	public class DeleteHandler : IHandler<DeleteExecution, bool>
	{
		private readonly IEntityRepository _entityRepository;

		public DeleteHandler(IEntityRepository entityRepository)
		{
			_entityRepository = entityRepository.NotNull(nameof(entityRepository));
		}

		public Task<bool> Execute(DeleteExecution deleteExecution, CancellationToken ct)
		{
			deleteExecution.NotNull(nameof(deleteExecution));

			return _entityRepository.Delete(deleteExecution.EntityDefinition, deleteExecution.Id, ct);
		}
	}
}