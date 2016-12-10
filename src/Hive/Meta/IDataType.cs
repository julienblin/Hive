using System;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;

namespace Hive.Meta
{
	public interface IDataType
	{
		string Name { get; }

		Type InternalNetType { get; }

		void ModelLoaded(IPropertyDefinition propertyDefinition);

		object ConvertToPropertyBagValue(IPropertyDefinition propertyDefinition, object value);

		object ConvertFromPropertyBagValue(IPropertyDefinition propertyDefinition, object value);

		Task SetDefaultValue(IPropertyDefinition propertyDefinition, IEntity entity, CancellationToken ct);
	}
}