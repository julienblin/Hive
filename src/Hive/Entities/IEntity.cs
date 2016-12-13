using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Foundation.Entities;
using Hive.Meta;
using NodaTime;

namespace Hive.Entities
{
	public interface IEntity : IDynamicMetaObjectProvider
	{
		IEntityDefinition Definition { get; }

		object Id { get; }

		object this[string propertyName] { get; set; }

		PropertyBag ToPropertyBag(bool keepRelationInfo = true);

		string Etag { get; set; }

		Instant? LastModified { get; set; }
	}
}