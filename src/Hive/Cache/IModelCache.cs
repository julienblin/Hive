namespace Hive.Cache
{
	public interface ICache<T>
	{
		T Get(string key);

		void Put(string key, T value);

		void Clear(string key);

		void Clear();
	}
}