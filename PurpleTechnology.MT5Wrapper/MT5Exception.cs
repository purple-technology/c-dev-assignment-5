using MetaQuotes.MT5CommonAPI;
using System;

namespace PurpleTechnology.MT5Wrapper
{
	public class MT5Exception : Exception
	{
		public MTRetCode ErrorCode { get; } = MTRetCode.MT_RET_OK;

		public MT5Exception()
		{

		}

		public MT5Exception(MTRetCode errorCode)
			: base()
		{
			ErrorCode = errorCode;
		}

		public MT5Exception(string message)
			: base(message)
		{

		}

		public MT5Exception(string message, Exception innerException)
			: base(message, innerException)
		{

		}

		public MT5Exception(string message, MTRetCode errorCode)
			: base(message)
		{
			ErrorCode = errorCode;
		}

		public MT5Exception(string message, MTRetCode errorCode, Exception innerException)
			: base(message, innerException)
		{
			ErrorCode = errorCode;
		}
	}
}
