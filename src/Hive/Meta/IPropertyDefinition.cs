using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Entities;

namespace Hive.Meta
{
	public interface IPropertyDefinition
	{
		IEntityDefinition EntityDefinition { get; }

		string Name { get; }

		string Description { get; }

		IDataType PropertyType { get; }

		object DefaultValue { get; }

		IEnumerable<IPropertyValidatorDefinition> ValidatorDefinitions { get; }

		IDictionary<string, object> AdditionalProperties { get; }

		PropertyBag PropertyBag { get; set; }

		Task SetDefaultValue(IEntity entity, CancellationToken ct);
	}
}