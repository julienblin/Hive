using Hive.Entities;

namespace Hive.ValueTypes
{
	public class RelationValueType : ValueType<IEntity>
	{
		public RelationValueType()
			: base("relation")
		{
		}
	}
}