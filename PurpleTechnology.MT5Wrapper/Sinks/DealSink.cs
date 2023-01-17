using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using Microsoft.Extensions.Logging;
using PurpleTechnology.MT5Wrapper.Abstractions;
using PurpleTechnology.MT5Wrapper.Data;
using PurpleTechnology.MT5Wrapper.Events;
using System;

namespace PurpleTechnology.MT5Wrapper.Sinks
{
	internal class DealSink : CIMTDealSink, IDealEventSource
	{
		private readonly ILogger<DealSink> _logger;
		private readonly CIMTManagerAPI _manager;
		private bool _disposed = false;

		#region IDealEventSource
		public event DealEvent DealAddEvent;
		#endregion IDealEventSource

		public DealSink(CIMTManagerAPI manager, ILogger<DealSink> logger)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_manager = manager ?? throw new ArgumentNullException(nameof(manager), "CIMTManagerAPI param cannot be null");
			var res = Initialize();
			if (MTRetCode.MT_RET_OK != res) {
				throw new TypeInitializationException(typeof(DealSink).AssemblyQualifiedName, null);
			}
		}

		private MTRetCode Initialize()
		{
			var res = RegisterSink();
			if (MTRetCode.MT_RET_OK != res) {
				_logger.LogError("Register deal sink failed: {Result}", res);
				return res;
			}

			res = _manager.DealSubscribe(this);
			if (MTRetCode.MT_RET_OK != res) {
				_logger.LogError("Subscribe deal sink failed: {Result}", res);
				return res;
			}

			return MTRetCode.MT_RET_OK;
		}

		protected override void Dispose(bool disposing)
		{
			if (_disposed) {
				return;
			}

			try {
				_manager.DealUnsubscribe(this);
				_disposed = true;

				base.Dispose(disposing);
			}
			catch (Exception ex) {
				_logger.LogError(ex, "Dispose DealSink failed");
			}
		}

		public override void OnDealAdd(CIMTDeal deal)
		{
			DealAddEvent?.Invoke(this, new DealEventArgs(new MT5Deal(deal)));
		}
	}
}
