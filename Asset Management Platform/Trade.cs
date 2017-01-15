using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{

    public class Trade
    {
        public string BuyOrSell;
        public Security Security;
        public string Ticker;
        public int Shares;
        public string Terms;
        public double Limit;
        public string OrderDuration;


        public Trade(string buyOrSell, Security securityToAdd, string ticker, int shares, string terms, double limit, string orderDuration)
        {
            BuyOrSell = buyOrSell;
            Security = securityToAdd;
            Ticker = ticker;
            Shares = shares;
            Terms = terms;
            Limit = limit;
            OrderDuration = orderDuration;

        }
        public Trade()
        {

        }
    }
}
