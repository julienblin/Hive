using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Hive.Data;
using Hive.Entities;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Options;
using Hive.Foundation.Extensions;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hive.Azure.DocumentDb
{
	public class DocumentDbEntityRepository : IEntityRepository
	{
		private readonly IOptions<DocumentDbOptions> _options;
		private readonly Lazy<IDocumentClient> _lazyClient;

		public DocumentDbEntityRepository(IOptions<DocumentDbOptions> options)
		{
			_options = options.NotNull(nameof(options));
			_lazyClient = new Lazy<IDocumentClient>(CreateClient);
			CollectionUri = UriFactory.CreateDocumentCollectionUri(_options.Value.Database, _options.Value.Collection);
			JsonSerializer = new JsonSerializer();
		}

		public async Task<TEntity> Create<TEntity, TId>(TEntity entity, CancellationToken ct)
			where TEntity : class, IEntity<TId>
		{
			entity.NotNull(nameof(entity));
			var doc = await Client.CreateDocumentAsync(CollectionUri, ConvertToDocument(entity));
			return ConvertToEntity<TEntity>(doc);
		}

		public async Task<TEntity> Update<TEntity, TId>(TEntity entity, CancellationToken ct)
			where TEntity : class, IEntity<TId>
		{
			entity.NotNull(nameof(entity));

			var updatedDoc = await Client.ReplaceDocumentAsync(GetDocumentUri(entity), ConvertToDocument(entity));
			return ConvertToEntity<TEntity>(updatedDoc);
		}

		public async Task<bool> Delete<TEntity, TId>(TId id, CancellationToken ct)
			where TEntity : class, IEntity<TId>
		{
			try
			{
				await Client.DeleteDocumentAsync(GetDocumentUri(id));
			}
			catch (DocumentClientException ex)
			{
				if (ex.StatusCode == HttpStatusCode.NotFound)
					return false;
				throw;
			}
			return true;
		}

		private object ConvertToDocument<TId>(IEntity<TId> entity)
		{
			var doc = JObject.FromObject(entity, JsonSerializer);
			doc[DocumentDbConstants.TypeKey] = entity.GetType().FullName;
			return doc;
		}

		private TEntity ConvertToEntity<TEntity>(ResourceResponse<Document> updatedDoc)
		{
			return JsonConvert.DeserializeObject<TEntity>(updatedDoc.ToString());
		}

		private Uri GetDocumentUri<TId>(IEntity<TId> entity)
		{
			return GetDocumentUri(entity.Id);
		}

		private Uri GetDocumentUri<TId>(TId id)
		{
			return UriFactory.CreateDocumentUri(_options.Value.Database, _options.Value.Collection, id.ToString());
		}

		private IDocumentClient Client => _lazyClient.Value;

		private Uri CollectionUri { get; }

		private JsonSerializer JsonSerializer { get; }

		private IDocumentClient CreateClient()
		{
			var client = new DocumentClient(_options.Value.ServiceEndpoint, _options.Value.AuthKey);
			CreateDatabaseIfNotExists(client).GetAwaiter().GetResult();
			CreateCollectionIfNotExists(client).GetAwaiter().GetResult();
			return client;
		}

		private async Task CreateDatabaseIfNotExists(IDocumentClient client)
		{
			try
			{
				await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_options.Value.Database));
			}
			catch (DocumentClientException e)
			{
				if (e.StatusCode == HttpStatusCode.NotFound)
					await client.CreateDatabaseAsync(new Database { Id = _options.Value.Database });
				else
					throw;
			}
		}

		private async Task CreateCollectionIfNotExists(IDocumentClient client)
		{
			try
			{
				await client.ReadDocumentCollectionAsync(CollectionUri);
			}
			catch (DocumentClientException e)
			{
				if (e.StatusCode == HttpStatusCode.NotFound)
					await client.CreateDocumentCollectionAsync(
						UriFactory.CreateDatabaseUri(_options.Value.Database),
						new DocumentCollection { Id = _options.Value.Collection },
						new RequestOptions { OfferThroughput = 1000 });
				else
					throw;
			}
		}
	}
}