using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Exceptions;
using Hive.Foundation.Extensions;
using Hive.Meta.Data;
using Hive.ValueTypes;

namespace Hive.Meta.Impl
{
	internal class PropertyDefinition : IPropertyDefinition, IOriginalDataHolder<PropertyDefinitionData>
	{
		private IDictionary<string, object> _properties = new Dictionary<string, object>();

		public IEntityDefinition EntityDefinition { get; set; }

		public string Name { get; set; }

		public IDataType PropertyType { get; set; }

		public object DefaultValue { get; set; }

		public Task SetDefaultValue(IEntity entity, CancellationToken ct)
		{
			return PropertyType.SetDefaultValue(this, entity, ct);
		}

		public T GetProperty<T>(string propertyName)
		{
			var result = _properties.SafeGet(propertyName);
			return result == null ? default(T) : (T)result;
		}

		public void SetProperty(string propertyName, object value)
		{
			_properties[propertyName] = value;
		}

		public PropertyDefinitionData OriginalData { get; set; }

		internal void FinishLoading(IValueTypeFactory valueTypeFactory)
		{
			if (PropertyType == null)
			{
				PropertyType = EntityDefinition.Model.EntitiesBySingleName.SafeGet(OriginalData.Type);
			}

			if (PropertyType == null)
			{
				throw new ModelLoadingException($"Unable to resolve property type {OriginalData.Type} for {this}.");
			}

			if (PropertyType is IValueType)
			{
				((IValueType) PropertyType).FinishLoading(valueTypeFactory, this);
			}
		}

		public override string ToString() => $"{EntityDefinition}.{Name}";
	}
}