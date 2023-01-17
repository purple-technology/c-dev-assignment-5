using PurpleTechnology.MT5Wrapper.Events;

namespace PurpleTechnology.MT5Wrapper.Abstractions
{
	public interface IDealEventSource
	{
		event DealEvent DealAddEvent;
	}
}
