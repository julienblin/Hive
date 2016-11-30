using System.IO;
using Hive.Foundation.Exceptions;
using Hive.Meta.Data.Impl;
using Xunit;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using System.Linq;
using Hive.Foundation.Extensions;

namespace Hive.Tests.Meta.Data.Impl
{
	public class JsonStructureMetaRepositoryTests
	{
		[Fact]
		public async Task ItShouldLoadAStructure()
		{
			var configService = new TestConfigService(JsonStructureMetaRepository.ModelsRootConfigValue, GetSampleStructureFilePath());
			var metaRepository = new JsonStructureMetaRepository(configService);

			var modelData = await metaRepository.GetModel("SampleModel", CancellationToken.None);
			modelData.Name.Should().Be("SampleModel");
			modelData.Version.Should().Be("1.0.0");

			modelData.Entities.Should().HaveCount(3);
			var clientEntity = modelData.Entities.First(x => x.SingleName.SafeOrdinalEquals("client"));
			clientEntity.PluralName.Should().Be("clients");
			clientEntity.Type.Should().Be("masterdata");
		}

		private string GetSampleStructureFilePath()
		{
			var location = typeof(JsonStructureMetaRepositoryTests).GetTypeInfo().Assembly.Location;
			var dirPath = Path.GetDirectoryName(location);
			return Path.Combine(dirPath, @"..\..\..\Root");
		}
	}
}