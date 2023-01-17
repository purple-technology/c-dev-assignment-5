using MetaQuotes.MT5CommonAPI;

namespace PurpleTechnology.MT5Wrapper.Data
{
	public enum MT5DealAction
	{
		Buy = 0,
		Sell = 1,
		Balance = 2,
		Credit = 3,
		Charge = 4,
		Correction = 5,
		Bonus = 6,
		Commission = 7,
		CommissionDaily = 8,
		CommissionMonthly = 9,
		AgentDaily = 10,
		AgentMonthly = 11,
		InterestRate = 12,
		BuyCanceled = 13,
		SellCanceled = 14,
		Dividend = 15,
		DividendFranked = 16,
		Tax = 17,
		Agent = 18,
		SoCompensation = 19
	}

	public class MT5Deal
	{
		public ulong Login { get; set; }
		public string Symbol { get; set; }
		public MT5DealAction Action { get; set; }
		public ulong Order { get; set; }
		public long OpenTime { get; set; }
		public ulong Volume { get; set; }

		public MT5Deal() { }

		internal MT5Deal(CIMTDeal deal)
		{
			Login = deal.Login();
			Symbol = deal.Symbol();
			Action = (MT5DealAction)deal.Action();
			Order = deal.Order();
			OpenTime = deal.Time();
			Volume = deal.Volume();
		}
	}
}
