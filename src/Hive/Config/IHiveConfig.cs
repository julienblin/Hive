using System;
using System.Collections.Generic;

namespace Hive.Config
{
	public interface IHiveConfig
	{
		string EnvironmentName { get; }

		EnvironmentMode Mode { get; }

		TimeSpan DefaultTimeout { get; }

		T GetValue<T>(string name) where T : class;
	}
}