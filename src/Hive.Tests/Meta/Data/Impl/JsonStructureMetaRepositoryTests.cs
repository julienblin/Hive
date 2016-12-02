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
			var configService = new ConfigServiceMock(JsonStructureMetaRepository.ModelsRootConfigValue, GetSampleStructureFilePath());
			var metaRepository = new JsonStructureMetaRepository(configService);

			var modelData = await metaRepository.GetModel("SampleModel", CancellationToken.None);
			modelData.Name.Should().Be("SampleModel");
			modelData.Version.Should().Be("1.0.0");

			modelData.Entities.Should().HaveCount(3);
			var clientEntity = modelData.Entities.First(x => x.SingleName.SafeOrdinalEquals("client"));
			clientEntity.PluralName.Should().Be("clients");
			clientEntity.Type.Should().Be("masterdata");
			var firstNameProperty = clientEntity.Properties.FirstOrDefault(x => x.Name.SafeOrdinalEquals("firstName"));
			firstNameProperty.Should().NotBeNull();
			firstNameProperty.Type.Should().Be("string");

			var patientEntity = modelData.Entities.First(x => x.SingleName.SafeOrdinalEquals("patient"));
			patientEntity.PluralName.Should().Be("patients");
			var typeProperty = patientEntity.Properties.FirstOrDefault(x => x.Name.SafeOrdinalEquals("type"));
			typeProperty.Should().NotBeNull();
			typeProperty.Type.Should().Be("enum");
			var values = typeProperty.GetValue<string[]>("values");
			values.Should().NotBeNull();
			values.Should().Contain(new[] {"internal", "external"});
		}

		private string GetSampleStructureFilePath()
		{
			var location = typeof(JsonStructureMetaRepositoryTests).GetTypeInfo().Assembly.Location;
			var dirPath = Path.GetDirectoryName(location);
			return Path.Combine(dirPath, @"..\..\..\Root");
		}
	}
}