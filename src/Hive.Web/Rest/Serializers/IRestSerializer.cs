using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Entities;
using Hive.Meta;

namespace Hive.Web.Rest.Serializers
{
	public interface IRestSerializer
	{
		IEnumerable<string> MediaTypes { get; }

		void Serialize(object @object, Stream stream);

		PropertyBag Deserialize(Stream stream);
	}
}