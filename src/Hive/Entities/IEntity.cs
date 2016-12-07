using System.Threading;
using System.Threading.Tasks;
using Hive.Foundation.Entities;
using Hive.Meta;

namespace Hive.Entities
{
	public interface IEntity
	{
		IEntityDefinition Definition { get; }

		object Id { get; }

		PropertyBag ToPropertyBag();

		object this[string propertyName] { get; set; }

		Task Init(CancellationToken ct);
	}
}