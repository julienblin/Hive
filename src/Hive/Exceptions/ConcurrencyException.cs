using System;
using Hive.Entities;

namespace Hive.Exceptions
{
	public class ConcurrencyException : HiveException

	{
		public ConcurrencyException(IEntity entity, Exception innerException)
			: base($"Unable to update {entity} because of a conflict. You may want to fetch the entity again and re-apply the modification with an updated ETag.", innerException)
		{
			Entity = entity;
		}

		public IEntity Entity { get; set; }
	}
}