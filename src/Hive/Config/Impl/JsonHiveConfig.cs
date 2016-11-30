using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hive.Config.Impl
{
	internal class JsonHiveConfig : IHiveConfig
	{
		public JsonHiveConfig()
		{
			EnvironmentName = "Production";
			Mode = EnvironmentMode.Production;
			DefaultTimeout = TimeSpan.FromSeconds(30);
		}

		public string EnvironmentName { get; set; }

	    public EnvironmentMode Mode { get; set; }

		public TimeSpan DefaultTimeout { get; set; }

		[JsonExtensionData]
		public IDictionary<string, JToken> AdditionalData { get; set; }

		public T GetValue<T>(string name) where T : class
		{
			return AdditionalData.ContainsKey(name)
				? AdditionalData[name].ToObject<T>()
				: null;
		}
	}
}
