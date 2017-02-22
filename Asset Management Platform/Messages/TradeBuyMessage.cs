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
        public Taxlot Taxlot;

        public TradeBuyMessage(Trade trade, Taxlot taxlot)
        {
            Trade = trade;
            Taxlot = taxlot;
        }
    }
}
