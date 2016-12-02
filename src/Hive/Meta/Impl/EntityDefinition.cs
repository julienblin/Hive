﻿using System;
using System.Collections.Generic;
using Hive.Foundation.Extensions;
using Hive.Meta.Data;

namespace Hive.Meta.Impl
{
	internal class EntityDefinition : IEntityDefinition, IOriginalDataHolder<EntityDefinitionData>
	{
		public string Name => SingleName;

		public IModel Model { get; set; }

		public string SingleName { get; set; }

		public string PluralName { get; set; }

		public EntityType EntityType { get; set; }

		public IReadOnlyDictionary<string, IPropertyDefinition> Properties { get; set; }

		public EntityDefinitionData OriginalData { get; set; }

		internal void FinishLoading(IValueTypeFactory valueTypeFactory)
		{
			foreach (var propertyDefinition in Properties?.Values)
			{
				var pf = propertyDefinition as PropertyDefinition;
				if (pf != null)
				{
					pf.FinishLoading(valueTypeFactory);
				}
			}
		}

		public override string ToString() => $"{Model}.{Name}";
	}
}