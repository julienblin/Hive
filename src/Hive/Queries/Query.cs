using Hive.Context;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Queries
{
	public class Query
	{
		private readonly IContext _context;

		public Query(IContext context)
		{
			_context = context.NotNull(nameof(context));
		}

		public IDataType ResultType { get; }
	}
}