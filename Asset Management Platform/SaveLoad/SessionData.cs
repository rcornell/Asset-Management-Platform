using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.SaveLoad
{
    public class SessionData
    {
        public List<LimitOrder> LimitOrders;
        public List<Position> Positions;

        public SessionData()
        {
            
        }

        public SessionData(List<LimitOrder> limitOrders, List<Position> positions)
        {
            LimitOrders = limitOrders;
            Positions = positions;
        }
    }
}
