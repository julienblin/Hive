using Hive.Context;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Queries
{
	public class Query
	{
		private readonly IContext _context;
		private readonly IDataType _dataType;

		public Query(IContext context, IDataType dataType)
		{
			_context = context.NotNull(nameof(context));
			_dataType = dataType.NotNull(nameof(dataType));
		}
	}
}