using System;

namespace Hive.Commands
{
	public abstract class Command<T>
	{
		protected Command()
		{
			Id = Guid.NewGuid();
		}

		public Guid Id { get; }

		public Type ResultType => typeof(T);
	}
}