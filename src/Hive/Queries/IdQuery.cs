using Hive.Context;

namespace Hive.Queries
{
	public class IdQuery : Query
	{
		public IdQuery(IContext context, object id)
			: base(context)
		{
			Id = id;
		}

		public object Id { get; }
	}
}