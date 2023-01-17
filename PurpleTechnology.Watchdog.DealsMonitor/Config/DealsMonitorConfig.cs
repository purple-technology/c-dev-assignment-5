using System;

namespace PurpleTechnology.Watchdog.DealsMonitor.Config
{
	public class DealsMonitorConfig
	{
		public TimeSpan OpenTimeDelta { get; set; } = TimeSpan.FromSeconds(1);
		public decimal VolumeToBalanceRatio { get; set; } = 0.05m;
		public TimeSpan CacheTresholdTimeDelta { get; set; } = TimeSpan.FromSeconds(30);
	}
}
