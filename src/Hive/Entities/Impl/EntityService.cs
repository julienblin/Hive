using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Commands;
using Hive.Meta;
using Hive.Queries;

namespace Hive.Entities.Impl
{
	public class EntityService : IEntityService
	{
		public Task<T> Execute<T>(Query<T> query, CancellationToken ct)
		{
			throw new System.NotImplementedException();
		}

		public async Task<T> Execute<T>(Command<T> command, CancellationToken ct)
		{
			if (command is CreateCommand)
			{
				var createCommand = command as CreateCommand;
				return (T)createCommand.Entity;
			}

			throw new NotImplementedException();
		}
	}
}