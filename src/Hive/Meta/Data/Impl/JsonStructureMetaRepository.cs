using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hive.Config;
using Hive.Exceptions;
using Hive.Foundation;
using Hive.Foundation.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Hive.Meta.Data.Impl
{
	public class JsonStructureMetaRepository : IMetaRepository
	{
		private const string ManifestFilename = "manifest.json";
		private const string EntitiesFolderName = "Entities";

		private readonly IOptions<JsonStructureMetaRepositoryOptions> _options;
		private readonly JsonSerializer _serializer;

		public JsonStructureMetaRepository(IOptions<JsonStructureMetaRepositoryOptions> options)
		{
			_options = options.NotNull(nameof(options));
			_serializer = new HiveJsonSerializer(new PropertyDefinitionDataConverter());
		}

		public async Task<ModelData> GetModel(string modelName, CancellationToken ct)
		{
			var baseDirectory = GetBaseDirectory(modelName, ct);
			var modelData = LoadModelManifest(baseDirectory);
			modelData.Name = baseDirectory.Name;
			await LoadEntities(baseDirectory, modelData, ct);

			return modelData;
		}

		private DirectoryInfo GetBaseDirectory(string modelName, CancellationToken ct)
		{
			if (_options.Value.ModelsPath.IsNullOrEmpty())
			{
				throw new HiveConfigException(
					$"No value was provided in the options for {nameof(_options.Value.ModelsPath)}, which should point to a base folder for models definitions.");
			}

			var baseDirectory = new DirectoryInfo(Path.Combine(_options.Value.ModelsPath, modelName));
			if (!baseDirectory.Exists)
			{
				throw new HiveConfigException(
					$"Unable to find the path for the folder {baseDirectory} that should reprensents the model {modelName}.");
			}

			return baseDirectory;
		}

		private ModelData LoadModelManifest(DirectoryInfo baseDirectory)
		{
			var manifestFile = baseDirectory.GetFiles(ManifestFilename).FirstOrDefault();
			if (manifestFile == null)
			{
				throw new ModelLoadingException(
					$"Unable to find a manifest file named {ManifestFilename} in the {baseDirectory.FullName} directory.");
			}

			try
			{
				return _serializer.DeserializeFile<ModelData>(manifestFile.FullName);
			}
			catch (Exception ex)
			{
				throw new ModelLoadingException(
					$"There has been an error while loading model manifest file {manifestFile.FullName}.", ex);
			}
		}

		private async Task LoadEntities(DirectoryInfo baseDirectory, ModelData modelData, CancellationToken ct)
		{
			var entitiesDirectory = baseDirectory.GetDirectories(EntitiesFolderName).FirstOrDefault();
			if (entitiesDirectory == null) return;

			var entityFiles = entitiesDirectory.GetFiles("*.json", SearchOption.TopDirectoryOnly);

			try
			{
				modelData.Entities = (await entityFiles
					.SafeForEachParallel((x, token) => _serializer.DeserializeFileAsync<EntityDefinitionData>(x.FullName, token), ct)
					).ToArray();
			}
			catch (Exception ex)
			{
				throw new ModelLoadingException($"There has been an error while loading entities from the {entitiesDirectory.FullName} directory.", ex);
			}
		}
	}
}