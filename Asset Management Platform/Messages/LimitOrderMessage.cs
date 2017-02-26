using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class LimitOrderMessage
    {
        public List<LimitOrder> LimitOrders;
        public bool IsStartup;

        public LimitOrderMessage(List<LimitOrder> limitOrders, bool isStartup)
        {
            LimitOrders = limitOrders;
            IsStartup = isStartup;
        }
    }
}
