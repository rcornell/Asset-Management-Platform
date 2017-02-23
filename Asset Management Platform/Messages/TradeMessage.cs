using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class TradeMessage
    {
        public Trade Trade;

        public TradeMessage(Trade trade)
        {
            Trade = trade;
        }
    }
}
