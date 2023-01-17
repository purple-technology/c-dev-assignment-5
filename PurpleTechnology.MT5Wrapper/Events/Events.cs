using PurpleTechnology.MT5Wrapper.Data;

namespace PurpleTechnology.MT5Wrapper.Events
{
	public class DealEventArgs
	{
		public MT5Deal Deal { get; }

		public DealEventArgs(MT5Deal deal)
		{
			Deal = deal;
		}
	}

	public delegate void DealEvent(object source, DealEventArgs args);
}
