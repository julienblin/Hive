using System.Collections.Immutable;
using Hive.Entities;
using Hive.Handlers;

namespace Hive.Meta
{
	public interface IEntityDefinition : IDataType
	{
		string FullName { get; }

		IModel Model { get; }

		string SingleName { get; }

		string PluralName { get; }

		EntityType EntityType { get; }

		IImmutableDictionary<string, IPropertyDefinition> Properties { get; }

		ConcurrencyHandling ConcurrencyHandling { get; }

		IHandler<IEntity, IEntity> CreateHandler { get; }

		IHandler<IEntity, IEntity> UpdateHandler { get; }

		IHandler<DeleteExecution, bool> DeleteHandler { get; }
	}
}