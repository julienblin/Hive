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
		private readonly IOptions<DocumentDbOptions> _options;

		public DocumentDbEntityRepository(IOptions<DocumentDbOptions> options, IMetaService metaService)
		{
			_options = options.NotNull(nameof(options));
			_metaService = metaService.NotNull(nameof(metaService));
			_lazyClient = new Lazy<IDocumentClient>(CreateClient);
			_lazyCollectionUri = new Lazy<Uri>(CreateCollectionUri);
		}

		private IDocumentClient Client => _lazyClient.Value;

		private Uri CollectionUri => _lazyCollectionUri.Value;

		public Task<T> Execute<T>(Query<T> query, CancellationToken ct)
		{
			if (query is IdQuery)
			{
				return ExecuteIdQuery<T>(query as IdQuery, ct);
			}
			throw new NotImplementedException();
		}

		private async Task<T> ExecuteIdQuery<T>(IdQuery query, CancellationToken ct)
		{
			query.NotNull(nameof(query));
			query.Id.NotNull(nameof(query.Id));

			var doc = await Client.ReadDocumentAsync(CreateDocumentUri(query.Id.ToString()));
			if (doc == null)
				return default(T);

			return (T)ConvertToEntity(query.EntityDefinition, doc);
		}

		public async Task<IEntity> Create(IEntity entity, CancellationToken ct)
		{
			entity.NotNull(nameof(entity));
			var doc =
				await Client.CreateDocumentAsync(CollectionUri, ConvertToDocument(entity), disableAutomaticIdGeneration: true);
			return ConvertToEntity(entity.Definition, doc.Resource);
		}

		public Task<IEntity> Update(IEntity entity, CancellationToken ct)
		{
			throw new NotImplementedException();
		}

		public async Task Start(CancellationToken ct)
		{
			await CreateDatabaseIfNotExists();
			await CreateCollectionIfNotExists();
		}

		private static object ConvertToDocument(IEntity entity)
		{
			var propertyBag = entity.ToPropertyBag();
			propertyBag[DocumentDbConstants.EntityDefinitionProperty] = entity.Definition.FullName;
			return propertyBag;
		}

		private IEntity ConvertToEntity(IEntityDefinition entityDefinition, Document doc)
		{
			var propertyBag = HiveJsonSerializer.Instance.Deserialize<PropertyBag>(doc.ToString());
			return new Entity(entityDefinition, propertyBag);
		}

		private IDocumentClient CreateClient()
		{
			return new DocumentClient(_options.Value.ServiceEndpoint, _options.Value.AuthKey);
		}

		private Uri CreateCollectionUri()
		{
			return UriFactory.CreateDocumentCollectionUri(_options.Value.Database, _options.Value.Collection);
		}

		private Uri CreateDocumentUri(string documentId)
		{
			return UriFactory.CreateDocumentUri(_options.Value.Database, _options.Value.Collection, documentId);
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
	}
}