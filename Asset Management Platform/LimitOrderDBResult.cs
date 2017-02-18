using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public class LimitOrderDbResult
    {
        public string TradeType;
        public string Ticker;
        public int Shares;
        public decimal Limit;
        public string SecurityType;
        public string OrderDuration;

        public LimitOrderDbResult(string tradeType, string ticker, int shares, decimal limit, string secType, string duration)
        {
            TradeType = tradeType;
            Ticker = ticker;
            Shares = shares;
            Limit = limit;
            SecurityType = secType;
            OrderDuration = duration;
        }
    }
}
