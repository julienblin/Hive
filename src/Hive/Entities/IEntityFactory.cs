using System.Threading;
using System.Threading.Tasks;
using Hive.Foundation.Entities;
using Hive.Meta;

namespace Hive.Entities
{
	public interface IEntityFactory
	{
		Task<IEntity> Create(IEntityDefinition definition, CancellationToken ct);

		Task<IEntity> Create(IEntityDefinition definition, PropertyBag initialValues, CancellationToken ct);

		Task<IEntity> Hydrate(IEntityDefinition definition, PropertyBag initialValues, CancellationToken ct);
	}
}