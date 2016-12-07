using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta.Data;
using Hive.ValueTypes;

namespace Hive.Meta.Impl
{
	internal class EntityDefinition : IEntityDefinition, IOriginalDataHolder<EntityDefinitionData>
	{
		public string FullName => $"{Model.Name}.{Name}";

		public string Name => SingleName;

		public Type InternalNetType => typeof(IEntityDefinition);

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

		object IDataType.ConvertTo(IPropertyDefinition propertyDefinition, object value)
		{
			throw new NotImplementedException();
		}

		object IDataType.ConvertFrom(IPropertyDefinition propertyDefinition, object value)
		{
			throw new NotImplementedException();
		}

		public virtual Task SetDefaultValue(IPropertyDefinition propertyDefinition, IEntity entity, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public override string ToString() => FullName;
	}
}