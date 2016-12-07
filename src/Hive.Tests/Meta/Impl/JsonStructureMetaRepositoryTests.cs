using System.IO;
using Xunit;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using System.Linq;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta.Impl;
using Microsoft.Extensions.Options;

namespace Hive.Tests.Meta.Impl
{
	public class JsonStructureMetaRepositoryTests
	{
		[Fact]
		public async Task ItShouldLoadAModelAsPropertyBag()
		{
			var options = new JsonStructureMetaRepositoryOptions
			{
				ModelsPath = GetSampleStructureFilePath()
			};
			var metaRepository = new JsonStructureMetaRepository(new OptionsWrapper<JsonStructureMetaRepositoryOptions>(options));

			var modelData = await metaRepository.GetModel("SampleModel", CancellationToken.None);
			modelData["name"].Should().Be("SampleModel");
			modelData["version"].Should().Be("1.0.0");

			var entities = modelData["entities"] as PropertyBag[];
			entities.Should().HaveCount(3);
			var clientEntity = entities.First(x => ((string)x["singlename"]).SafeOrdinalEquals("client"));
			((string)clientEntity["pluralName"]).Should().Be("clients");
			((string)clientEntity["type"]).Should().Be("masterdata");
			var clientEntityProperties = clientEntity["properties"] as PropertyBag[];
			var firstNameProperty = clientEntityProperties.FirstOrDefault(x => ((string)x["name"]).SafeOrdinalEquals("firstName"));
			firstNameProperty.Should().NotBeNull();
			((string)firstNameProperty["type"]).Should().Be("string");

			var patientEntity = entities.First(x => ((string)x["singlename"]).SafeOrdinalEquals("patient"));
			((string)patientEntity["pluralName"]).Should().Be("patients");
			var patientEntityProperties = patientEntity["properties"] as PropertyBag[];
			var typeProperty = patientEntityProperties.FirstOrDefault(x => ((string)x["name"]).SafeOrdinalEquals("type"));
			typeProperty.Should().NotBeNull();
			((string)typeProperty["type"]).Should().Be("enum");
			var values = typeProperty["values"] as string[];
			values.Should().NotBeNull();
			values.Should().Contain(new[] { "internal", "external" });
		}

		private string GetSampleStructureFilePath()
		{
			var location = typeof(JsonStructureMetaRepositoryTests).GetTypeInfo().Assembly.Location;
			var dirPath = Path.GetDirectoryName(location);
			return Path.Combine(dirPath, @"..\..\..\Root");
		}
	}
}