namespace PurpleTechnology.MT5Wrapper.Abstractions
{
	public interface IMT5ApiFactory<TServerId>
	{
		IMT5Api<TServerId> Create();
	}
}
