using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hive.Config;
using Hive.Foundation;
using Hive.Foundation.Exceptions;
using Hive.Foundation.Extensions;

namespace Hive.Meta.Data.Impl
{
	public class JsonStructureMetaRepository : IMetaRepository
	{
		public const string ModelsRootConfigValue = "modelsRoot";

		private const string ManifestFilename = "manifest.json";
		private const string EntitiesFolderName = "Entities";

		private readonly IConfigService _configService;

		public JsonStructureMetaRepository(IConfigService configService)
		{
			_configService = configService.NotNull(nameof(configService));
		}

		public async Task<ModelData> GetModel(string modelName, CancellationToken ct)
		{
			var baseDirectory = await GetBaseDirectory(modelName, ct);
			var modelData = LoadModelManifest(baseDirectory);
			modelData.Name = baseDirectory.Name;
			await LoadEntities(baseDirectory, modelData, ct);

			return modelData;
		}

		private async Task<DirectoryInfo> GetBaseDirectory(string modelName, CancellationToken ct)
		{
			var config = await _configService.GetConfig(ct);
			var folderValue = config.GetValue<string>(ModelsRootConfigValue);
			if (folderValue.IsNullOrEmpty())
			{
				throw new HiveConfigException(
					$"No value was provided in the configuration for the property {ModelsRootConfigValue}, which should point to a base folder for models definitions.");
			}

			var baseDirectory = new DirectoryInfo(Path.Combine(folderValue, modelName));
			if (!baseDirectory.Exists)
			{
				throw new HiveConfigException(
					$"Unable to find the path for the folder {baseDirectory} that should reprensents the model {modelName}.");
			}

			return baseDirectory;
		}

		private static ModelData LoadModelManifest(DirectoryInfo baseDirectory)
		{
			var manifestFile = baseDirectory.GetFiles(ManifestFilename).FirstOrDefault();
			if (manifestFile == null)
			{
				throw new ModelLoadingException(
					$"Unable to find a manifest file named {ManifestFilename} in the {baseDirectory.FullName} directory.");
			}

			try
			{
				return HiveJsonSerializer.Instance.Deserialize<ModelData>(manifestFile.FullName);
			}
			catch (Exception ex)
			{
				throw new ModelLoadingException(
					$"There has been an error while loading model manifest file {manifestFile.FullName}.", ex);
			}
		}

		private static async Task LoadEntities(DirectoryInfo baseDirectory, ModelData modelData, CancellationToken ct)
		{
			var entitiesDirectory = baseDirectory.GetDirectories(EntitiesFolderName).FirstOrDefault();
			if (entitiesDirectory == null) return;

			var entityFiles = entitiesDirectory.GetFiles("*.json", SearchOption.TopDirectoryOnly);

			try
			{
				modelData.Entities = (await entityFiles.SafeForEachParallel(LoadFile<EntityDefinitionData>, ct)).ToArray();
			}
			catch (Exception ex)
			{
				throw new ModelLoadingException($"There has been an error while loading entities from the {entitiesDirectory.FullName} directory.", ex);
			}
		}

		private static Task<T> LoadFile<T>(FileInfo fileInfo, CancellationToken ct)
		{
			return Task.Run(
				() => HiveJsonSerializer.Instance.Deserialize<T>(fileInfo.FullName),
				ct
			);
		}
	}
}