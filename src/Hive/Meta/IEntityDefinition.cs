using System.Collections.Immutable;
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

		ICreateHandler CreateHandler { get; }

		IUpdateHandler UpdateHandler { get; }

		IDeleteHandler DeleteHandler { get; }
	}
}