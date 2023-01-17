using MetaQuotes.MT5CommonAPI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PurpleTechnology.MT5Wrapper.Abstractions;
using PurpleTechnology.MT5Wrapper.Events;
using PurpleTechnology.MT5Wrapper.Sinks;
using System;

namespace PurpleTechnology.MT5Wrapper
{
	internal class MT5Api<TServerId> : IMT5Api<TServerId>
	{
		private bool _disposed = false;
		private readonly ILogger<MT5Api<TServerId>> _logger;
		private readonly ManagerSink _managerSink;
		private readonly DealSink _dealSink;

		public event DealEvent DealAddEvent;
		public TServerId ServerId { get; private set; }

		public MT5Api(IServiceProvider serviceProvider)
		{
			_logger = serviceProvider.GetRequiredService<ILogger<MT5Api<TServerId>>>();
			_managerSink = new ManagerSink(serviceProvider.GetRequiredService<ILogger<ManagerSink>>());
			_dealSink = new DealSink(_managerSink.ManagerAPI, serviceProvider.GetRequiredService<ILogger<DealSink>>());
			_dealSink.DealAddEvent += OnDealAdd;
		}

		public void Dispose()
		{
			if (_disposed) {
				return;
			}

			try {
				_dealSink.DealAddEvent -= OnDealAdd;
				_dealSink.Dispose();
				_managerSink.Dispose();
				_disposed = true;
			}
			catch (Exception ex) {
				_logger.LogError(ex, "Dispose MT5Api{ServerId} failed", ServerId);
			}
		}

		private void OnDealAdd(object _, DealEventArgs args)
		{
			DealAddEvent?.Invoke(this, args);
		}

		public bool Connect(MT5ServerConnectionConfig<TServerId> connectionParams)
		{
			ServerId = connectionParams.ServerId;
			return _managerSink.Connect(connectionParams);
		}

		public void Disconnect()
		{
			ServerId = default;
			_managerSink.Disconnect();
		}

		// WARNING: This method cannot be called from event handlers
		public decimal GetUserBalance(ulong login)
		{
			using var account = _managerSink.ManagerAPI.UserCreateAccount();
			if (null == account) {
				throw new MT5Exception($"Create user account object for user {login}@{ServerId} failed");
			}
			var retCode = _managerSink.ManagerAPI.UserAccountRequest(login, account);
			if (MTRetCode.MT_RET_OK != retCode) {
				throw new MT5Exception($"Get user account for user {login}@{ServerId} failed", retCode);
			}
			return Convert.ToDecimal(account.Balance());
		}
	}
}
