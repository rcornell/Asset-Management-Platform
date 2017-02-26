using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    class PositionPricingMessage
    {
        public List<Security> PricedSecurities;
        public bool IsStartup;

        public PositionPricingMessage(List<Security> pricedSecurities, bool isStartup)
        {
            PricedSecurities = pricedSecurities;
            IsStartup = isStartup;
        }
    }
}
