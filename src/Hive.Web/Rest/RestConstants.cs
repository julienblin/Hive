namespace Hive.Web.Rest
{
	public static class RestConstants
	{
		public const string ReservedOperatorsPrefix = "$";
		public const string LimitOperator = ReservedOperatorsPrefix + "limit";
		public const string OrderOperator = ReservedOperatorsPrefix + "orderby";

		public const string ContinuationTokenHeader = "X-Query-Continuation";
	}
}