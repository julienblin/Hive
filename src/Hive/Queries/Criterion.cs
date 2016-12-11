using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Queries
{
	public class Criterion : ICriterion
	{
		public static ICriterion IdEq(object id)
		{
			return Eq(MetaConstants.IdProperty, id);
		}

		public static ICriterion Eq(string propertyName, object value)
		{
			return new Criterion(propertyName, Operators.Eq, value);
		}

		public static ICriterion In(string propertyName, object[] value)
		{
			return new Criterion(propertyName, Operators.In, value);
		}

		public Criterion(string propertyName, string @operator, object value = null)
		{
			PropertyName = propertyName.NotNullOrEmpty(nameof(propertyName));
			Operator = @operator.NotNull(nameof(@operator));
			Value = value;
		}

		public string PropertyName { get; set; }

		public string Operator { get; set; }

		public object Value { get; set; }

		public bool IsIdCriterion
			=> Operator.SafeOrdinalEquals(Operators.Eq) && PropertyName.SafeOrdinalEquals(MetaConstants.IdProperty);
	}
}