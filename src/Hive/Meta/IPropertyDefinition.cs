﻿using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Meta.Data;

namespace Hive.Meta
{
	public interface IPropertyDefinition
	{
		IEntityDefinition EntityDefinition { get; }

		string Name { get; }

		IDataType PropertyType { get; }

		T GetProperty<T>(string propertyName);

		void SetProperty(string propertyName, object value);

		object DefaultValue { get; }

		Task SetDefaultValue(IEntity entity, CancellationToken ct);
	}
}