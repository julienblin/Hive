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

		void SetPropertyValue(string propertyName, object value);

		T GetPropertyValue<T>(string propertyName);

		bool HasPropertyValue(string propertyName);

		Task Init(CancellationToken ct);
	}
}