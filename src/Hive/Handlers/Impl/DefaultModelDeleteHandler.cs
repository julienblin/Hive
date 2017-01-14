﻿using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;

namespace Hive.Handlers.Impl
{
	public class DefaultModelDeleteHandler<T> : IHandleDelete<T>
		where T: class, IEntity
	{
		public Task<IHandlerResult> Delete(T resource, CancellationToken ct)
		{
			throw new System.NotImplementedException();
		}
	}
}