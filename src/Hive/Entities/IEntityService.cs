using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Commands;
using Hive.Meta;
using Hive.Queries;

namespace Hive.Entities
{
	public interface IEntityService
	{
		Task<IEnumerable<IDataType>> Execute(Query query, CancellationToken ct);

		Task Execute(Command command, CancellationToken ct);
	}
}