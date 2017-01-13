namespace Hive.Models
{
	public interface IModel
	{
	}

	public interface IModel<out T> : IModel
	{
		T Id { get; }
	}
}