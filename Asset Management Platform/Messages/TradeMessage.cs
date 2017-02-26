using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{

    //This class is created in the ViewModel and Sent to PortfolioManagementService
    public class TradeMessage
    {
        public Trade Trade;

        public TradeMessage(Trade trade)
        {
            Trade = trade;
        }
    }
}
