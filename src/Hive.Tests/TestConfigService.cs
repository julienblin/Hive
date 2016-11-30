using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Config;
using Hive.Foundation.Extensions;

namespace Hive.Tests
{
	public class TestConfigService : IConfigService
	{
		private readonly IHiveConfig _config;

		public TestConfigService(string key, string value)
			: this(new TestHiveConfig(key, value))
		{
		}

		public TestConfigService(IReadOnlyDictionary<string, object> values = null)
			: this(new TestHiveConfig(values))
		{
		}

		public TestConfigService(IHiveConfig config)
		{
			_config = config.NotNull(nameof(config));
		}

		public Task<IHiveConfig> GetConfig(CancellationToken ct)
		{
			return Task.FromResult(_config);
		}
	}
}