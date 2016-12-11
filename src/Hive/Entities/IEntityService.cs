using System.Threading;
using System.Threading.Tasks;
using Hive.Commands;
using Hive.Meta;
using Hive.Queries;

namespace Hive.Entities
{
	public interface IEntityService
	{
		IQuery CreateQuery(IEntityDefinition entityDefinition);

		Task<T> Execute<T>(Command<T> command, CancellationToken ct);
	}
}