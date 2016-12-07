using System;
using Hive.Cache;
using Hive.Meta;

namespace Hive.Tests.Mocks
{
	public class ModelCacheMock : IModelCache
	{
		private readonly Action<IModel> _putAsserts;
		private IModel _returnedValue;

		public ModelCacheMock(IModel returnedValue = null, Action<IModel> putAsserts = null)
		{
			_returnedValue = returnedValue;
			_putAsserts = putAsserts;
		}

		public IModel Get(string modelName)
		{
			return _returnedValue;
		}

		public void Put(IModel model)
		{
			_putAsserts?.Invoke(model);
		}

		public void Clear(string key)
		{
			_returnedValue = null;
		}

		public void Clear()
		{
			_returnedValue = null;
		}
	}
}