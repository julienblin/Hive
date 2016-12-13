namespace Hive.Web.Rest
{
	public static class RestConstants
	{
		public const string ReservedOperatorsPrefix = "$";
		public const string LimitOperator = ReservedOperatorsPrefix + "limit";
		public const string OrderOperator = ReservedOperatorsPrefix + "orderby";
		public const string IncludeOperator = ReservedOperatorsPrefix + "include";
		public const string SelectOperator = ReservedOperatorsPrefix + "select";

		public const string ContinuationTokenHeader = "X-Query-Continuation";
	}
}