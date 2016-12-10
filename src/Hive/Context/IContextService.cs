namespace Hive.Context
{
	public interface IContextService
	{
		IContext StartContext();

		IContext GetContext();

		void StopContext();
	}
}