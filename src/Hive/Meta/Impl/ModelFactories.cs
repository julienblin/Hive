using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Validation;
using Hive.ValueTypes;

namespace Hive.Meta.Impl
{
	internal class ModelFactories : IModelFactories
	{
		public ModelFactories(
			IValueTypeFactory valueTypeFactory,
			IValidatorFactory validatorFactory,
			IEntityFactory entityFactory)
		{
			ValueType = valueTypeFactory.NotNull(nameof(valueTypeFactory));
			Validator = validatorFactory.NotNull(nameof(validatorFactory));
			Entity = entityFactory.NotNull(nameof(entityFactory));
		}

		public IValueTypeFactory ValueType { get; }

		public IValidatorFactory Validator { get; }

		public IEntityFactory Entity { get; }
	}
}