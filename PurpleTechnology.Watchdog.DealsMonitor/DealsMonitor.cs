using Microsoft.Extensions.Logging;
using PurpleTechnology.Watchdog.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PurpleTechnology.Watchdog.DealsMonitor
{
	public class DealsMonitor<TServerId> : IMonitor
	{
		private readonly ILogger<DealsMonitor<TServerId>> _logger;

		public DealsMonitor(ILogger<DealsMonitor<TServerId>> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		public Task StartMonitoring(CancellationToken stoppingToken)
		{
			// TODO: implement suspicious deals monitoring
			return Task.CompletedTask;
		}
	}
}
