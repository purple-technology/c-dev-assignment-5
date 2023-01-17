using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PurpleTechnology.MT5Wrapper;
using PurpleTechnology.Watchdog.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PurpleTechnology.Watchdog.Service
{
	internal sealed class WatchdogService<TServerId> : BackgroundService
	{
		private readonly ILogger<WatchdogService<TServerId>> _logger;
		private readonly IHostApplicationLifetime _hostApplicationLifetime;
		private readonly IEnumerable<IMonitor> _monitors;
		private bool _disposed = false;

		public WatchdogService(
			IHostApplicationLifetime hostApplicationLifetime,
			IEnumerable<IMonitor> monitors,
			ILogger<WatchdogService<TServerId>> logger)
		{
			_logger = logger;
			_hostApplicationLifetime = hostApplicationLifetime ?? throw new ArgumentNullException(nameof(hostApplicationLifetime));
			_monitors = monitors ?? throw new ArgumentNullException(nameof(monitors));
		}

		protected override async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			var tasks = _monitors
				.Select(Monitor => Monitor.StartMonitoring(cancellationToken))
				.ToList();
			var task = Task.WhenAny(tasks);

			try {
				await task;
			}
			catch (MT5Exception ex) {
				_logger.LogError(ex, "Application MT5 exception occured");
			}
			catch (Exception ex) {
				_logger.LogError(ex, "Application exception occured");
			}
			finally {
				if (task.IsFaulted) {
					_logger.LogInformation("Stopping application");
					_hostApplicationLifetime.StopApplication();
				}
			}

			var all = Task.WhenAll(tasks);
			try {
				await all;
			}
			catch (Exception) {
				if (null != all.Exception?.InnerExceptions && all.Exception.InnerExceptions.Any()) {
					foreach (var ex in all.Exception.InnerExceptions) {
						_logger.LogError(ex, "Monitor exception occured");
					}
				}
			}

			foreach (var t in tasks) {
				t.Dispose();
			}
			tasks.Clear();
		}

		public override void Dispose()
		{
			if (_disposed) {
				return;
			}

			_disposed = true;

			base.Dispose();
		}
	}
}
