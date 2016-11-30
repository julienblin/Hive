using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Hive.Foundation
{
	public class HiveJsonSerializer : JsonSerializer
	{
		static HiveJsonSerializer()
		{
			Instance = new HiveJsonSerializer();
		}

		public static HiveJsonSerializer Instance { get; }

		private HiveJsonSerializer()
		{
			Converters.Add(new StringEnumConverter());
			ContractResolver = new CamelCasePropertyNamesContractResolver();
		}
	}
}