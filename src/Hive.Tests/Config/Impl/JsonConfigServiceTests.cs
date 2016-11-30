using System;
using System.Collections.Generic;
using System.IO;
using Hive.Config.Impl;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Hive.Config;
using Hive.Foundation.Exceptions;

namespace Hive.Tests.Config.Impl
{
	public class JsonConfigServiceTests
	{
		[Fact]
		public async Task ItShouldLoadConfig()
		{
			var configService = new JsonConfigService(GetSampleConfigFilePath());
			var config = await configService.GetConfig(CancellationToken.None);

			config.EnvironmentName.Should().Be("Tests");
			config.Mode.Should().Be(EnvironmentMode.Debug);
			config.DefaultTimeout.Should().Be(TimeSpan.FromMinutes(1));
			config.GetValue<string>("AdditionalString").Should().Be("Foo");
			var additionalDict = config.GetValue<Dictionary<string, string>>("AdditionalDict");
			additionalDict.Should().NotBeNull();
			additionalDict.Should().ContainKeys("Foo", "Bar");
		}

		[Fact]
		public void ItShouldThrowHiveFatalExceptions()
		{
			var configService = new JsonConfigService(Path.GetRandomFileName());
			configService.Awaiting(x => x.GetConfig(CancellationToken.None))
				.ShouldThrow<HiveFatalException>();
		}

		private string GetSampleConfigFilePath()
		{
			var location = typeof(JsonConfigServiceTests).GetTypeInfo().Assembly.Location;
			var dirPath = Path.GetDirectoryName(location);
			return Path.Combine(dirPath, @"..\..\..\Config\Impl\SampleConfig.json");
		}
	}
}