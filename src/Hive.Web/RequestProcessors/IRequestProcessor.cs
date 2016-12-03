using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Hive.Web.RequestProcessors
{
	public interface IRequestProcessor
	{
		Task<bool> Process(HttpContext context, CancellationToken ct);
	}
}