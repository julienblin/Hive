using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Hive.Config;
using Hive.Foundation.Extensions;

namespace Hive.Tests.Mocks
{
	public class ConfigServiceMock : IConfigService
	{
		private readonly IHiveConfig _config;

		public ConfigServiceMock(string key, string value)
			: this(new HiveConfigMock(key, value))
		{
		}

		public ConfigServiceMock(IReadOnlyDictionary<string, object> values = null)
			: this(new HiveConfigMock(values))
		{
		}

		public ConfigServiceMock(IHiveConfig config)
		{
			_config = config.NotNull(nameof(config));
		}

		public Task<IHiveConfig> GetConfig(CancellationToken ct)
		{
			return Task.FromResult(_config);
		}
	}
}