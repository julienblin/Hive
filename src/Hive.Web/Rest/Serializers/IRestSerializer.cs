using System.Collections.Generic;
using System.IO;
using Hive.Entities;

namespace Hive.Web.Rest.Serializers
{
	public interface IRestSerializer
	{
		IEnumerable<string> MediaTypes { get; }

		void Serialize(object @object, Stream stream);
	}
}