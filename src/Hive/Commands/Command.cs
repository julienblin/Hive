using System;
using Hive.Context;
using Hive.Foundation.Extensions;

namespace Hive.Commands
{
	public abstract class Command
	{
		protected Command(IContext context)
		{
			Id = Guid.NewGuid();
			Context = context.NotNull(nameof(context));
		}

		public Guid Id { get; }

		public IContext Context { get; }
	}
}