using Hive.Meta;

namespace Hive.Entities
{
	public interface IEntity
	{
		IEntityDefinition Definition { get; }
	}
}