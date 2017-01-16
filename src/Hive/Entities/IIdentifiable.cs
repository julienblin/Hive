namespace Hive.Entities
{
	public interface IIdentifiable<out TId>
	{
		TId Id { get; }
	}
}