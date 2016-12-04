using System;

namespace Hive.Queries
{
	public abstract class Query<T> : IQuery
	{
		public Type ResultType => typeof(T);
	}
}