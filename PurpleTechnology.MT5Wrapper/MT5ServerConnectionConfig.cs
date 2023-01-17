using System;

namespace PurpleTechnology.MT5Wrapper
{
	public class MT5ServerConnectionConfig<TServerId>
	{
		public TServerId ServerId { get; set; }
		public string IP { get; set; }
		public ulong Login { get; set; }
		public string Password { get; set; }
		public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);
	}
}
