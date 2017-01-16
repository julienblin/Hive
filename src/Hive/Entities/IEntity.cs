namespace Hive.Entities
{
	public interface IEntity<out TId> : IIdentifiable<TId>
	{
	}
}