using System;
using Hive.Queries;
using Hive.ValueTypes;

namespace Hive.Exceptions
{
	public class QueryException : HiveFatalException
	{
		public QueryException(IQuery query, string message) : base(message)
		{
			Query = query;
		}

		public QueryException(IQuery query, string message, Exception inner) : base(message, inner)
		{
			Query = query;
		}

		public IQuery Query { get; set; }
	}
}