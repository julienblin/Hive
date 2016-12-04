using System.Threading;
using System.Threading.Tasks;
using Hive.Commands;
using Hive.Queries;

namespace Hive.Entities
{
	public interface IEntityService
	{
		Task<T> Execute<T>(Query<T> query, CancellationToken ct);

		Task<T> Execute<T>(Command<T> command, CancellationToken ct);
	}
}