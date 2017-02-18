using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public class ExchangeTradedFund : Security
    {

        public double Bid;
        public double Ask;
        public int Volume;
        public int BidSize;
        public int AskSize;

        public ExchangeTradedFund(string cusip, string ticker, string description, decimal lastPrice, double yield)
            : base (cusip, ticker, description, lastPrice, yield)
        {
            SecurityType = "Exchange Traded Fund";
        }

        public ExchangeTradedFund(string cusip, string ticker, string description, decimal lastPrice, double yield, double bid, double ask, int volume, int bidSize, int askSize)
            : base(cusip, ticker, description, lastPrice, yield)
        {
            SecurityType = "Exchange Traded Fund";

            Bid = bid;
            Ask = ask;
            Volume = volume;
            BidSize = bidSize;
            AskSize = askSize;
        }
    }
}
