using System;
using Hive.Entities;

namespace Hive.Meta
{
	public interface IDataType
	{
		string Name { get; }

		Type InternalNetType { get; }

		object ConvertValue(IPropertyDefinition propertyDefinition, object value);
	}
}