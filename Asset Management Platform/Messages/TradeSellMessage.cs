using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    //This class is created in PortfolioManagementService and sent to PortfolioDatabaseService
    public class TradeSellMessage
    {
        public Trade Trade;
        public Taxlot Taxlot;
        public bool SellingAllShares;
        public bool SellingPartialShares;

        public TradeSellMessage(Trade trade, Taxlot taxlot, bool allShares, bool partialShares)
        {
            Trade = trade;
            Taxlot = taxlot;
            SellingAllShares = allShares;
            SellingPartialShares = partialShares;
        }

        public TradeSellMessage(Taxlot taxlot, bool allShares, bool partialShares)
        {
            Taxlot = taxlot;
            SellingAllShares = allShares;
            SellingPartialShares = partialShares;
        }

        public TradeSellMessage(Trade trade, bool allShares, bool partialShares)
        {
            Trade = trade;
            SellingAllShares = allShares;
            SellingPartialShares = partialShares;
        }
    }
}
