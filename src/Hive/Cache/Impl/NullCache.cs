namespace Hive.Cache.Impl
{
	public class NullCache<T> : ICache<T>
	{
		public T Get(string key)
		{
			return default(T);
		}

		public void Put(string key, T value)
		{
		}

		public void Clear(string key)
		{
		}

		public void Clear()
		{
		}
	}
}