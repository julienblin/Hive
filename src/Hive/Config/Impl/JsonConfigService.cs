using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hive.Exceptions;
using Hive.Foundation;
using Hive.Foundation.Extensions;
using Hive.Foundation.Lifecycle;

namespace Hive.Config.Impl
{
	public class JsonConfigService : IConfigService, IStartable
	{
		private readonly string _path;
		private readonly Lazy<IHiveConfig> _lazyConfig;

		public JsonConfigService(string path)
		{
			_path = path.NotNullOrEmpty(nameof(path));
			_lazyConfig = new Lazy<IHiveConfig>(LoadConfig);
		}

		public Task<IHiveConfig> GetConfig(CancellationToken ct)
		{
			return Task.FromResult(_lazyConfig.Value);
		}

		private IHiveConfig LoadConfig()
		{
			try
			{
				using (var stream = File.OpenRead(_path))
				{
					return HiveJsonSerializer.Instance.Deserialize<JsonHiveConfig>(stream);
				}
			}
			catch (Exception ex)
			{
				throw new HiveConfigException("There has been an error while loading configuration", ex);
			}
		}

		Task IStartable.Start(CancellationToken ct)
		{
			var config = _lazyConfig.Value; // Forces initialization.
			return Task.CompletedTask;
		}
	}
}