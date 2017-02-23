using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class TradeCompleteMessage
    {
        public List<Position> Positions;
        public List<Taxlot> Taxlots;
        public bool TradeSuccess;

        public TradeCompleteMessage(List<Position> positions, List<Taxlot> taxlots, bool tradeSuccess)
        {
            Positions = positions;
            Taxlots = taxlots;
            TradeSuccess = tradeSuccess;
        }
    }
}
