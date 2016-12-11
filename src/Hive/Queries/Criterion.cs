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
			return new Criterion(propertyName, Operators.Eq, value, propertyName.SafeOrdinalEquals(MetaConstants.IdProperty));
		}

		public static ICriterion In(string propertyName, object[] value)
		{
			return new Criterion(propertyName, Operators.In, value);
		}

		public Criterion(string propertyName, string @operator, object value = null, bool isIdCriterion = false)
		{
			PropertyName = propertyName.NotNullOrEmpty(nameof(propertyName));
			Operator = @operator.NotNull(nameof(@operator));
			Value = value;
			IsIdCriterion = isIdCriterion;
		}

		public string PropertyName { get; set; }

		public string Operator { get; set; }

		public object Value { get; set; }

		public bool IsIdCriterion { get; set; }
	}
}