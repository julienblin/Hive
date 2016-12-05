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
				await InitDefaultValues(createCommand.Entity, ct);
				return (T)createCommand.Entity;
			}

			throw new NotImplementedException();
		}

		private async Task InitDefaultValues(IEntity entity, CancellationToken ct)
		{
			foreach (var propertyDefinition in entity.Definition.Properties.Values)
			{
				if ((propertyDefinition.DefaultValue != null) && !entity.HasPropertyValue(propertyDefinition.Name))
				{
					await propertyDefinition.SetDefaultValue(entity, ct);
				}
			}
		}
	}
}