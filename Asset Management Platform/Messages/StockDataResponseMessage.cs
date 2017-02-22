using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class StockDataResponseMessage
    {
        public List<Security> Securities;
        public Security Security;

        public StockDataResponseMessage(List<Security> securities)
        {
            Securities = securities;
        }

        public StockDataResponseMessage(Security security)
        {
            Security = security;
        }
    }
}
