using System.IO;
using Hive.Foundation;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;

namespace Hive.Web.Rest.Serializers.Impl
{
	public class JsonRestSerializer : RestSerializer
	{
		public JsonRestSerializer()
			: base(new[] {"application/json"})
		{
		}

		public override void Serialize(object message, Stream stream)
		{
			HiveJsonSerializer.Instance.Serialize(message, stream);
		}

		public override PropertyBag Deserialize(Stream stream)
		{
			return HiveJsonSerializer.Instance.Deserialize<PropertyBag>(stream);
		}
	}
}