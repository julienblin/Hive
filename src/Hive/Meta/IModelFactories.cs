using Hive.Entities;
using Hive.Validation;
using Hive.ValueTypes;

namespace Hive.Meta
{
	public interface IModelFactories
	{
		IValueTypeFactory ValueType { get; }

		IValidatorFactory Validator { get; }

		IEntityFactory Entity { get; }
	}
}