using System;
using System.Linq;
using Hive.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hive.Foundation.Entities.Converters
{
	public class PropertyBagJsonConverter : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			serializer.Serialize(writer, ((PropertyBag) value).ToDictionary(x => x.Key, x => x.Value));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			return GetPropertyBag(JObject.Load(reader));
		}

		private PropertyBag GetPropertyBag(JObject jobject)
		{
			var bag = new PropertyBag();
			foreach (var property in jobject.Properties())
				bag[property.Name] = GetWhiteListedObject(property.Value);

			return bag;
		}

		private object GetWhiteListedObject(JToken token)
		{
			switch (token.Type)
			{
				case JTokenType.None:
				case JTokenType.Constructor:
				case JTokenType.Property:
				case JTokenType.Comment:
				case JTokenType.Null:
				case JTokenType.Undefined:
				case JTokenType.Raw:
					return null;
				case JTokenType.Object:
					return GetPropertyBag((JObject) token);
				case JTokenType.Array:
					var jArray = (JArray) token;
					if (!jArray.HasValues)
						return null;

					var innerValues = jArray.Select(GetWhiteListedObject).Where(x => x != null).ToArray();
					if (innerValues.Length == 0)
						return null;

					var firstResultType = innerValues[0].GetType();
					if (innerValues.Any(x => x.GetType() != firstResultType))
						throw new SerializationException($"Mixed type arrays are not supported ({string.Join(", ", innerValues)}).");
					var result = Array.CreateInstance(firstResultType, innerValues.Length);
					Array.Copy(innerValues, result, innerValues.Length);
					return result;
				case JTokenType.Integer:
					return token.Value<int>();
				case JTokenType.Float:
					return token.Value<float>();
				case JTokenType.String:
				case JTokenType.Date:
				case JTokenType.Uri:
				case JTokenType.TimeSpan:
					return token.Value<string>();
				case JTokenType.Guid:
					return token.Value<Guid>();
				case JTokenType.Boolean:
					return token.Value<bool>();
				case JTokenType.Bytes:
					return token.Value<byte[]>();
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(PropertyBag);
		}
	}
}