using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Hive.Web.Rest
{
	public interface IRestRequestProcessor
	{
		Task<HttpResponse> Process(HttpRequest request, CancellationToken ct);
	}
}