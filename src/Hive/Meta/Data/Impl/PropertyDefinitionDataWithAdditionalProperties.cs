using System.Collections.Generic;
using System.Linq;
using Hive.Foundation.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hive.Meta.Data.Impl
{
	internal class PropertyDefinitionDataWithAdditionalProperties : PropertyDefinitionData
	{
		[JsonExtensionData]
		public IDictionary<string, JToken> AdditionalData { get; set; }

		public override T GetValue<T>(string propertyName)
		{
			var token = AdditionalData.SafeGet(propertyName);
			return token == null ? default(T) : token.ToObject<T>();
		}
	}
}