using System;

namespace Hive.Config
{
	public class HiveOptions
	{
		public string EnvironmentName { get; set; }

		public EnvironmentMode Mode { get; set; }

		public TimeSpan DefaultTimeout { get; set; }
	}
}