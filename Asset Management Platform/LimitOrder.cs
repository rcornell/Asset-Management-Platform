using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public class LimitOrder
    {

        public string TradeType;
        public string Ticker;
        public decimal Shares;
        public decimal Limit;
        public Security SecurityType;
        public string OrderDuration;

        public LimitOrder()
        {

        }

        public LimitOrder(Trade trade)
        {
            TradeType = trade.BuyOrSell;
            Ticker = trade.Ticker;
            Shares = trade.Shares;
            Limit = trade.Limit;
            SecurityType = trade.Security;
            OrderDuration = trade.OrderDuration;
        }

        public override string ToString()
        {
            return string.Format(@"{0} {1} shares of {2} at LIMIT {3}.", TradeType, Shares, Ticker, Limit);
        }

        public bool IsLimitOrderActive(decimal lastPrice)
        {
            if (TradeType == "Buy" && lastPrice < Limit)
                return true;
            else if (TradeType == "Buy" && lastPrice > Limit)
                return false;
            else if (TradeType == "Sell" && lastPrice > Limit)
                return true;
            else if (TradeType == "Sell" && lastPrice < Limit)
                return false;

            return false; //Why did you reach this?
        }


    }
}
