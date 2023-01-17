using PurpleTechnology.MT5Wrapper.Abstractions;
using System;

namespace PurpleTechnology.MT5Wrapper
{
	public class MT5ApiFactory<TServerId> : IMT5ApiFactory<TServerId>
	{
		private readonly IServiceProvider _serviceProvider;
		public MT5ApiFactory(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public IMT5Api<TServerId> Create()
		{
			return new MT5Api<TServerId>(_serviceProvider);
		}
	}
}
