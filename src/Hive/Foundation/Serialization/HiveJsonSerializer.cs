﻿using Hive.Foundation.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Hive.Foundation
{
	public sealed class HiveJsonSerializer : JsonSerializer
	{
		static HiveJsonSerializer()
		{
			Instance = new HiveJsonSerializer();
		}

		public HiveJsonSerializer()
		{
			Converters.Add(new StringEnumConverter());
			ContractResolver = new CamelCasePropertyNamesContractResolver();
			DateParseHandling = DateParseHandling.None;
		}

		public HiveJsonSerializer(params JsonConverter[] converters)
			: this()
		{
			foreach (var jsonConverter in converters.Safe())
				Converters.Add(jsonConverter);
		}

		public static HiveJsonSerializer Instance { get; }
	}
}