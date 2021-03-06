﻿using System.Threading;
using System.Threading.Tasks;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Hive.Queries;

namespace Hive.Entities
{
	public static class EntityServiceExtensions
	{
		public static Task<IEntity> GetById(this IEntityService entityService, IEntityDefinition entityDefinition, object id, CancellationToken ct)
		{
			entityService.NotNull(nameof(entityService));
			entityDefinition.NotNull(nameof(entityDefinition));
			id.NotNull(nameof(id));

			return entityService.CreateQuery(entityDefinition).Add(Criterion.IdEq(id)).UniqueResult<IEntity>(ct);
		}

		public static Task<bool> Delete(this IEntityService entityService, IEntity entity, CancellationToken ct)
		{
			entityService.NotNull(nameof(entityService));
			entity.NotNull(nameof(entity));

			return entityService.Delete(entity.Definition, entity.Id, ct);
		}
	}
}