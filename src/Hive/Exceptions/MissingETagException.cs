using Hive.Entities;

namespace Hive.Exceptions
{
	public class MissingETagException : HiveException
	{
		public MissingETagException(IEntity entity)
			: base($"Missing Etag on entity {entity}. Impossible to update because it has been configured for optimistic concurrency.")
		{
			Entity = entity;
		}

		public IEntity Entity { get; set; }
	}
}