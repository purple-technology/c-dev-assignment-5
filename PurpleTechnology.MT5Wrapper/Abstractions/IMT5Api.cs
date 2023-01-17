using System;

namespace PurpleTechnology.MT5Wrapper.Abstractions
{
	public interface IMT5Api<TServerId> : IDealEventSource, IDisposable
	{
		TServerId ServerId { get; }

		bool Connect(MT5ServerConnectionConfig<TServerId> connectionParams);
		void Disconnect();

		decimal GetUserBalance(ulong login);
	}
}
