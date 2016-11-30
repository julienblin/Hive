using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Hive.Foundation
{
	public class HiveJsonSerializerSettings : JsonSerializerSettings
	{
		static HiveJsonSerializerSettings()
		{
			Instance = new HiveJsonSerializerSettings();
		}

		public static HiveJsonSerializerSettings Instance { get; }

		private HiveJsonSerializerSettings()
		{
			Converters.Add(new StringEnumConverter());
			ContractResolver = new CamelCasePropertyNamesContractResolver();
		}
	}
}