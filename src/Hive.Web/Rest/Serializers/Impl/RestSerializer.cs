using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;

namespace Hive.Web.Rest.Serializers.Impl
{
	public abstract class RestSerializer : IRestSerializer
	{
		protected RestSerializer(IEnumerable<string> mediaTypes)
		{
			MediaTypes = mediaTypes.NotNull(nameof(mediaTypes));
		}

		public IEnumerable<string> MediaTypes { get; }

		public abstract void Serialize(object @object, Stream stream);

		public abstract PropertyBag Deserialize(Stream stream);
	}
}