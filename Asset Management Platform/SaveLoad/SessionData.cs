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
        public List<Taxlot> Taxlots;

        public SessionData()
        {
            LimitOrders = new List<LimitOrder>();
            Taxlots = new List<Taxlot>();
        }

        public SessionData(List<LimitOrder> limitOrders, List<Taxlot> taxlots)
        {
            LimitOrders = limitOrders;
            Taxlots = taxlots;
        }
    }
}
