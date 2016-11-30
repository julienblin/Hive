using System.Threading;
using System.Threading.Tasks;

namespace Hive.Events
{
	public interface IHandle<in T>
		where T : IEvent
	{
		Task Handle(T @event, CancellationToken ct);
	}
}