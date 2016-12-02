using System;

namespace Hive.Context
{
	public interface IContext
	{
		Guid OperationId { get; }
	}
}