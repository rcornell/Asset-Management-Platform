using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class TradeMessage
    {
        public string Message;

        public string Ticker;

        public decimal Shares;

        public TradeMessage()
        {

        }

        public TradeMessage(string ticker, int shares)
        {

        }
    }
}
