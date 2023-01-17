using System.Threading;
using System.Threading.Tasks;

namespace PurpleTechnology.Watchdog.Abstractions
{
	public interface IMonitor
	{
		Task StartMonitoring(CancellationToken stoppingToken);
	}
}
