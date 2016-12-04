using System;
using Hive.Cache;

namespace Hive.Tests.Mocks
{
	public class CacheMock<T> : ICache<T>
	{
		private T _returnedValue;
		private readonly Action<string, T> _putAsserts;

		public CacheMock(T returnedValue = default(T), Action<string, T> putAsserts = null)
		{
			_returnedValue = returnedValue;
			_putAsserts = putAsserts;
		}

		public T Get(string modelName)
		{
			return _returnedValue;
		}

		public void Put(string key, T value)
		{
			_putAsserts?.Invoke(key, value);
		}

		public void Clear(string key)
		{
			_returnedValue = default(T);
		}

		public void Clear()
		{
			_returnedValue = default(T);
		}
	}
}