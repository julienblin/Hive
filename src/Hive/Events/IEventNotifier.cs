using System.Threading;
using System.Threading.Tasks;

namespace Hive.Events
{
	public interface IEventNotifier
	{
		Task Notify(IEvent @event, CancellationToken ct);
	}
}