using Hive.Executions;

namespace Hive.Handlers
{
	public interface IHandler<in TIn, TOut> : IExecution<TIn, TOut>
	{
		
	}
}