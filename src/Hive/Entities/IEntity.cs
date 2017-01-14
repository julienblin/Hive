namespace Hive.Entities
{
	public interface IEntity
	{
	}

	public interface IEntity<out T> : IEntity
	{
		T Id { get; }
	}
}