using System;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Meta.Impl;

namespace Hive.Meta
{
	public interface IDataType
	{
		string Name { get; }

		Type InternalNetType { get; }

		object ConvertValue(IPropertyDefinition propertyDefinition, object value);

		Task SetDefaultValue(IPropertyDefinition propertyDefinition, IEntity entity, CancellationToken ct);
	}
}