using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Entities;
using Hive.Meta.Impl;

namespace Hive.Meta
{
	public interface IPropertyDefinition
	{
		IEntityDefinition EntityDefinition { get; }

		string Name { get; }

		IDataType PropertyType { get; }

		object DefaultValue { get; }

		IDictionary<string, object> AdditionalProperties { get; }

		Task SetDefaultValue(IEntity entity, CancellationToken ct);

		PropertyBag PropertyBag { get; set; }
	}
}