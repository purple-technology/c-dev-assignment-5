using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PurpleTechnology.MT5Wrapper;
using PurpleTechnology.MT5Wrapper.Abstractions;
using PurpleTechnology.Watchdog.Abstractions;
using PurpleTechnology.Watchdog.Config;
using PurpleTechnology.Watchdog.DealsMonitor;
using PurpleTechnology.Watchdog.DealsMonitor.Config;
using Serilog;
using System;

namespace PurpleTechnology.Watchdog.Service
{
	public static class WatchdogApp
	{
		public static void Main(string[] args)
		{
			try {
				using var host = CreateHostBuilder(args).Build();
				host.Run();
			}
			catch (OperationCanceledException) {
				Log.Information("Application execution canceled");
			}
			catch (Exception e) {
				Log.Fatal(e, "Application terminated unexpectedly");
			}
		}

		private static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.UseSerilog(ConfigureLogger)
				.UseWindowsService()
				.ConfigureServices(ConfigureServices<string>);

		private static void ConfigureLogger(HostBuilderContext context, LoggerConfiguration loggerConfiguration) =>
			loggerConfiguration.ReadFrom.Configuration(context.Configuration);

		private static void ConfigureServices<TServerId>(HostBuilderContext context, IServiceCollection services) =>
			services
				.AddSingleton((sp) => {
					var config = context.Configuration
						.GetSection("DealsMonitorConfig")
						.Get<DealsMonitorConfig>();
					return config ?? throw new Exception("Missing Deals monitor config");
				})
				.AddSingleton((sp) => {
					var config = context.Configuration
							.GetSection("ServersConfig")
							.Get<ServersConfig<TServerId>>();
					return config ?? throw new Exception("Missing servers config");
				})
				.AddSingleton<IMT5ApiFactory<TServerId>, MT5ApiFactory<TServerId>>()
				.AddTransient<IMonitor, DealsMonitor<TServerId>>()
				.AddHostedService<WatchdogService<TServerId>>();
	}
}
