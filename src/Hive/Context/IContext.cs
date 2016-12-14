namespace Hive.Context
{
	public interface IContext
	{
		string OperationId { get; }

		ClientApplication ClientApplication { get; }
	}
}