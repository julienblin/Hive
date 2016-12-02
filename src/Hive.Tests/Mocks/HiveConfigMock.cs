﻿using System;
using System.Collections.Generic;
using Hive.Config;

namespace Hive.Tests.Mocks
{
	public class HiveConfigMock : IHiveConfig
	{
		private readonly IReadOnlyDictionary<string, object> _values;

		public HiveConfigMock(string key, string value)
			: this (new Dictionary<string, object> { { key, value } })
		{
		}

		public HiveConfigMock(IReadOnlyDictionary<string, object> values = null)
		{
			EnvironmentName = "Tests";
			Mode = EnvironmentMode.Debug;
			DefaultTimeout = TimeSpan.FromSeconds(5);
			_values = values ?? new Dictionary<string, object>();
		}

		public string EnvironmentName { get; set; }

		public EnvironmentMode Mode { get; set; }

		public TimeSpan DefaultTimeout { get; set; }

		public T GetValue<T>(string name) where T : class
		{
			return (T) _values[name];
		}
	}
}