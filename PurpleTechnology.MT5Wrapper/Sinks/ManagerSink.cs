using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;

namespace PurpleTechnology.MT5Wrapper.Sinks
{
	internal class ManagerSink : CIMTManagerSink
	{
		private readonly ILogger<ManagerSink> _logger;
		private bool _disposed = false;

		public CIMTManagerAPI ManagerAPI { get; private set; }

		public ManagerSink(ILogger<ManagerSink> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			var res = Initialize();
			if (MTRetCode.MT_RET_OK != res) {
				ManagerAPI?.Unsubscribe(this);
				ManagerAPI?.Release();
				SMTManagerAPIFactory.Shutdown();
				throw new TypeInitializationException(typeof(ManagerSink).AssemblyQualifiedName, null);
			}
		}

		private string GetMTDllPath()
		{
			var exePath = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
			var dirPath = Path.GetDirectoryName(exePath.AbsolutePath);
			var dllPath = $@"{dirPath}\";
			return dllPath;
		}

		private MTRetCode Initialize()
		{
			// Initialize the factory
			var res = SMTManagerAPIFactory.Initialize(GetMTDllPath());
			if (MTRetCode.MT_RET_OK != res) {
				_logger.LogError("SMTManagerAPIFactory.Initialize failed: {Result}", res);
				return res;
			}
			// Receive the API version 
			res = SMTManagerAPIFactory.GetVersion(out uint version);
			if (MTRetCode.MT_RET_OK != res) {
				_logger.LogError("SMTManagerAPIFactory.GetVersion failed: {Result}", res);
				return res;
			}
			// Check API version
			if (version != SMTManagerAPIFactory.ManagerAPIVersion) {
				_logger.LogError("Manager API version mismatch - {ManagerApiVersion}!={FactoryManagerApiVersion}", version, SMTManagerAPIFactory.ManagerAPIVersion);
				return MTRetCode.MT_RET_ERROR;
			}
			// Create new manager
			ManagerAPI = SMTManagerAPIFactory.CreateManager(version, out res);
			if (MTRetCode.MT_RET_OK != res) {
				_logger.LogError("SMTManagerAPIFactory.CreateManager failed: {Result}", res);
				return res;
			}
			if (null == ManagerAPI) {
				_logger.LogError("SMTManagerAPIFactory.CreateManager succeeded, but ManagerAPI is null");
				return MTRetCode.MT_RET_ERR_MEM;
			}
			//
			res = RegisterSink();
			if (MTRetCode.MT_RET_OK != res) {
				_logger.LogError("CIMTManagerSink.RegisterSink failed: {Result}", res);
				return res;
			}
			// Subscribe for events
			res = ManagerAPI.Subscribe(this);
			if (MTRetCode.MT_RET_OK != res) {
				_logger.LogError("CIMTManagerAPI.Subscribe failed: {Result}", res);
				return res;
			}

			_logger.LogDebug("Using ManagerAPI v{ManagerApiVersion}", version);

			return MTRetCode.MT_RET_OK;
		}

		#region CIMTManagerSink

		protected override void Dispose(bool disposing)
		{
			if (_disposed) {
				return;
			}

			try {
				ManagerAPI?.Unsubscribe(this);
				Disconnect();
				ManagerAPI?.Release();
				SMTManagerAPIFactory.Shutdown();

				_disposed = true;

				base.Dispose(disposing);
			}
			catch (Exception ex) {
				_logger.LogError(ex, "Dispose ManagerSink failed");
			}
		}

		#endregion // CIMTManagerSink

		public bool Connect<TServerId>(MT5ServerConnectionConfig<TServerId> config)
		{
			if (_disposed) {
				throw new ObjectDisposedException(GetType().FullName);
			}

			if (null == config) {
				throw new ArgumentNullException(nameof(config), "ConnectionParams cannot be null");
			}

			var res = ManagerAPI.Connect(config.IP, config.Login, config.Password, null, CIMTManagerAPI.EnPumpModes.PUMP_MODE_FULL, (uint)config.ConnectionTimeout.TotalMilliseconds);

			if (MTRetCode.MT_RET_OK == res) {
				_logger.LogDebug("Connected to {ServerIP}", config.IP);
				return true;
			}
			else {
				_logger.LogError("Connect to {ServerIP} failed: {Result}", config.IP, res);
				return false;
			}
		}

		public void Disconnect()
		{
			if (_disposed) {
				throw new ObjectDisposedException(GetType().FullName);
			}

			ManagerAPI.Disconnect();
		}
	}
}
