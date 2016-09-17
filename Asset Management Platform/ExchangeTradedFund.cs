using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    class ExchangeTradedFund : Security
    {

        public double Bid;
        public double Ask;
        public double Beta;
        public int Volume;
        public int BidSize;
        public int AskSize;

        public ExchangeTradedFund(string ticker, string description, double lastPrice, double yield)
            : base ( ticker, description, lastPrice, yield)
        {
            SecurityType = "Exchange Traded Fund";
        }

        public ExchangeTradedFund(string ticker, string description, double lastPrice, double yield, double bid, double ask, double beta, int volume, int bidSize, int askSize)
            : base(ticker, description, lastPrice, yield)
        {
            SecurityType = "Exchange Traded Fund";

            Bid = bid;
            Ask = ask;
            Beta = beta;
            Volume = volume;
            BidSize = bidSize;
            AskSize = askSize;
        }
    }
}
