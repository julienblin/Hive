using System;

namespace Hive.Context
{
	public class Context : IContext
	{
		public Context()
			: this(Guid.NewGuid())
		{
		}

		public Context(Guid operationId)
		{
			OperationId = operationId;
		}

		public Guid OperationId { get; }
	}
}