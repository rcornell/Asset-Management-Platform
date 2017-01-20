using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public class LimitOrderDBResult
    {

        //var tradeType = reader.GetString(1);
        //var ticker = reader.GetString(2);
        //var shares = reader.GetInt32(3);
        //var limit = reader.GetDecimal(4);
        //var securityType = reader.GetString(5);
        //var orderDuration = reader.GetString(6);

        public string TradeType;
        public string Ticker;
        public int Shares;
        public decimal Limit;
        public string SecurityType;
        public string OrderDuration;


        public LimitOrderDBResult(string tradeType, string ticker, int shares, decimal limit, string secType, string duration)
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
