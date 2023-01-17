using System.Collections.Generic;
using PurpleTechnology.MT5Wrapper;

namespace PurpleTechnology.Watchdog.Config
{
	public class ServersConfig<TServerId>
	{
		public IReadOnlyCollection<MT5ServerConnectionConfig<TServerId>> MT5ServersConfig { get; set; }
	}
}
