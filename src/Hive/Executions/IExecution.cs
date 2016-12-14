using System.Threading;
using System.Threading.Tasks;

namespace Hive.Executions
{
	public interface IExecution<in TIn, TOut>
	{
		Task<TOut> Execute(TIn arg, CancellationToken ct);
	}
}