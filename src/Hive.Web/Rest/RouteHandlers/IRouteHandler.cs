using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Hive.Web.Rest.RouteHandlers
{
	public interface IRouteHandler
	{
		Task Handle(HttpContext context);
	}
}