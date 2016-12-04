using System.Collections.Generic;

namespace Hive.Web.Rest.Serializers
{
	public interface IRestSerializerFactory
	{
		IRestSerializer GetByMediaType(string mediaType);

		IRestSerializer GetByMediaType(IEnumerable<string> mediaTypes);
	}
}