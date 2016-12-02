using System.Threading;
using System.Threading.Tasks;
using Hive.Entities;
using Hive.Foundation.Extensions;
using Hive.Meta;
using Microsoft.AspNetCore.Http;

namespace Hive.Web.Rest.Impl
{
	public class RestRequestProcessor : IRestRequestProcessor
	{
		private readonly IMetaService _metaService;
		private readonly IEntityService _entityService;

		public RestRequestProcessor(IMetaService metaService, IEntityService entityService)
		{
			_metaService = metaService.NotNull(nameof(metaService));
			_entityService = entityService.NotNull(nameof(entityService));
		}

		public Task<HttpResponse> Process(HttpRequest request, CancellationToken ct)
		{
			throw new System.NotImplementedException();
		}
	}
}