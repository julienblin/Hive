using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hive.Meta.Data.Impl
{
	public class PropertyDefinitionDataConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var result = new PropertyDefinitionDataWithAdditionalProperties();
			serializer.Populate(reader, result);
			return result;
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(PropertyDefinitionData);
		}

		public override bool CanWrite => false;
	}
}