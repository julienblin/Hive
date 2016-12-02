namespace Hive.Meta
{
	public interface IOriginalDataHolder<out T>
	{
		T OriginalData { get; }
	}
}