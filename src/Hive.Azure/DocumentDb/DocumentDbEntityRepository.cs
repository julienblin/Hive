using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation;
using Hive.Foundation.Entities;
using Hive.Foundation.Extensions;
using Hive.Foundation.Lifecycle;
using Hive.Meta;
using Hive.Queries;
using Hive.Telemetry;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Options;

namespace Hive.Azure.DocumentDb
{
	public class DocumentDbEntityRepository : IEntityRepository, IStartable
	{
		private readonly Lazy<IDocumentClient> _lazyClient;
		private readonly Lazy<Uri> _lazyCollectionUri;
		private readonly IMetaService _metaService;
		private readonly IEntityFactory _entityFactory;
		private readonly IOptions<DocumentDbOptions> _options;

		public DocumentDbEntityRepository(
			IOptions<DocumentDbOptions> options,
			ITelemetry telemetry,
			IMetaService metaService,
			IEntityFactory entityFactory)
		{
			_options = options.NotNull(nameof(options));
			Telemetry = telemetry.NotNull(nameof(telemetry));
			_metaService = metaService.NotNull(nameof(metaService));
			_entityFactory = entityFactory.NotNull(nameof(entityFactory));
			_lazyClient = new Lazy<IDocumentClient>(CreateClient);
			_lazyCollectionUri = new Lazy<Uri>(CreateCollectionUri);

			DependencyName = $"DocumentDb ({_options.Value.Database})";
		}

		internal IDocumentClient Client => _lazyClient.Value;

		internal ITelemetry Telemetry { get; }

		internal Uri CollectionUri => _lazyCollectionUri.Value;

		public IQuery CreateQuery(IEntityDefinition entityDefinition)
		{
			entityDefinition.NotNull(nameof(entityDefinition));

			return new DocumentDbQuery(this, entityDefinition);
		}

		public async Task<IEntity> Create(IEntity entity, CancellationToken ct)
		{
			entity.NotNull(nameof(entity));
			var doc = await Telemetry.TrackAsyncDependency(
				ct,
				_ => Client.CreateDocumentAsync(CollectionUri, ConvertToDocument(entity), disableAutomaticIdGeneration: true),
				DependencyKind.HTTP,
				DependencyName
			);
			return ConvertToEntity(entity.Definition, doc.Resource);
		}

		public async Task<IEntity> Update(IEntity entity, CancellationToken ct)
		{
			entity.NotNull(nameof(entity));
			var doc = await Telemetry.TrackAsyncDependency(
				ct,
				_ => Client.ReplaceDocumentAsync(GetDocumentUri(entity), ConvertToDocument(entity)),
				DependencyKind.HTTP,
				DependencyName
			);
			return ConvertToEntity(entity.Definition, doc.Resource);
		}

		public async Task<bool> Delete(IEntityDefinition entityDefinition, object id, CancellationToken ct)
		{
			entityDefinition.NotNull(nameof(entityDefinition));
			id.NotNull(nameof(id));

			try
			{
				await Telemetry.TrackAsyncDependency(
					ct,
					_ => Client.DeleteDocumentAsync(GetDocumentUri(entityDefinition, id)),
					DependencyKind.HTTP,
					DependencyName
				);
			}
			catch (DocumentClientException ex)
			{
				if (ex.StatusCode == HttpStatusCode.NotFound)
					return false;
				throw;
			}
			return true;
		}

		public async Task Start(CancellationToken ct)
		{
			await CreateDatabaseIfNotExists();
			await CreateCollectionIfNotExists();
		}

		internal object ConvertToDocument(IEntity entity)
		{
			//TODO: Poor man's version. Should implement it's own serialization mechanism.
			var propertyBag = entity.ToPropertyBag(keepRelationInfo: false);
			propertyBag[MetaConstants.IdProperty] = GetDocumentId(entity.Definition, propertyBag[MetaConstants.IdProperty]);
			propertyBag[DocumentDbConstants.EntityDefinitionProperty] = entity.Definition.FullName;

			return propertyBag;
		}

		internal IEntity ConvertToEntity(IEntityDefinition entityDefinition, Document doc)
		{
			var propertyBag = HiveJsonSerializer.Instance.Deserialize<PropertyBag>(doc.ToString());
			propertyBag[MetaConstants.IdProperty] =
				(propertyBag[MetaConstants.IdProperty] as string)?.Substring(entityDefinition.FullName.Length + 1);
			var entity = _entityFactory.Hydrate(entityDefinition, propertyBag);
			entity.Etag = doc.ETag;
			return entity;
		}

		private IDocumentClient CreateClient()
		{
			return new DocumentClient(_options.Value.ServiceEndpoint, _options.Value.AuthKey);
		}

		private Uri CreateCollectionUri()
		{
			return UriFactory.CreateDocumentCollectionUri(_options.Value.Database, _options.Value.Collection);
		}

		internal Uri GetDocumentUri(IEntity entity)
		{
			return GetDocumentUri(entity.Definition, entity.Id);
		}

		internal Uri GetDocumentUri(IEntityDefinition entityDefinition, object id)
		{
			return GetDocumentUri(GetDocumentId(entityDefinition, id));
		}

		internal Uri GetDocumentUri(string documentId)
		{
			return UriFactory.CreateDocumentUri(_options.Value.Database, _options.Value.Collection, documentId);
		}

		internal string GetDocumentId(IEntityDefinition entityDefinition, object id)
		{
			return $"{entityDefinition.FullName}_{id}";
		}

		private async Task CreateDatabaseIfNotExists()
		{
			try
			{
				await Client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_options.Value.Database));
			}
			catch (DocumentClientException e)
			{
				if (e.StatusCode == HttpStatusCode.NotFound)
					await Client.CreateDatabaseAsync(new Database {Id = _options.Value.Database});
				else
					throw;
			}
		}

		private async Task CreateCollectionIfNotExists()
		{
			try
			{
				await Client.ReadDocumentCollectionAsync(CollectionUri);
			}
			catch (DocumentClientException e)
			{
				if (e.StatusCode == HttpStatusCode.NotFound)
					await Client.CreateDocumentCollectionAsync(
						UriFactory.CreateDatabaseUri(_options.Value.Database),
						new DocumentCollection {Id = _options.Value.Collection},
						new RequestOptions {OfferThroughput = 1000});
				else
					throw;
			}
		}

		internal string DependencyName { get; } 
	}
}