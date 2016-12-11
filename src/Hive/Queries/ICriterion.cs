namespace Hive.Queries
{
	public interface ICriterion
	{
		string PropertyName { get; set; }

		object Value { get; set; }

		string Operator { get; set; }

		bool IsIdCriterion { get; }
	}
}