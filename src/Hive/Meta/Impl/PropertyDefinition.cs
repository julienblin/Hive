using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Exceptions;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.ValueTypes;

namespace Hive.Meta.Impl
{
	internal class PropertyDefinition : IPropertyDefinition
	{
		private readonly Lazy<IDictionary<string, object>> _additionalProperties
			= new Lazy<IDictionary<string, object>>(() => new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase));

		public IEntityDefinition EntityDefinition { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public IDataType PropertyType { get; set; }

		public object DefaultValue { get; set; }

		public IDictionary<string, object> AdditionalProperties => _additionalProperties.Value;

		public Task SetDefaultValue(IEntity entity, CancellationToken ct)
		{
			return PropertyType.SetDefaultValue(this, entity, ct);
		}

		public PropertyBag PropertyBag { get; set; }

		internal void ModelLoaded()
		{
			if (PropertyType == null)
			{
				if (PropertyBag == null)
				{
					throw new ModelLoadingException($"Unable to finish model loading because PropertyBag is null for {this}.");
				}
				PropertyType = EntityDefinition.Model.EntitiesBySingleName.SafeGet(PropertyBag["type"] as string);
			}

			if (PropertyType == null)
				throw new ModelLoadingException($"Unable to resolve property type {PropertyBag["type"] as string} for {this}.");

			PropertyType.ModelLoaded(this);
		}

		public override string ToString() => $"{EntityDefinition}.{Name}";
	}
}