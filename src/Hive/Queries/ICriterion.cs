namespace Hive.Queries
{
	public interface ICriterion
	{
		string PropertyName { get; set; }

		object Value { get; set; }

		object Operator { get; set; }

		bool IsIdCriterion { get; set; }
	}
}