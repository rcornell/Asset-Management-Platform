using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class TradeBuyMessage
    {
        public Trade Trade;

        public TradeBuyMessage(Trade trade)
        {
            Trade = trade;
        }
    }
}
