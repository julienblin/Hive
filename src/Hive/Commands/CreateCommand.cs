﻿using Hive.Context;
using Hive.Entities;
using Hive.Foundation.Extensions;

namespace Hive.Commands
{
	public class CreateCommand : Command
	{
		private readonly IEntity _entity;

		public CreateCommand(IContext context, IEntity entity)
			: base(context)
		{
			_entity = entity.NotNull(nameof(entity));
		}
	}
}