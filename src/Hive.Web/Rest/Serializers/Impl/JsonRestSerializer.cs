using System;
using System.IO;
using Hive.Foundation;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Web.Exceptions;

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
			try
			{
				return HiveJsonSerializer.Instance.Deserialize<PropertyBag>(stream);
			}
			catch (Exception ex)
			{
				throw new BadRequestException("Malformed json.", ex);
			}
		}
	}
}