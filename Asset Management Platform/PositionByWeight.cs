using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public class PositionByWeight
    {

        public string Ticker;

        public decimal Weight;

        public PositionByWeight(string ticker, decimal weight)
        {
            Ticker = ticker;
            Weight = weight;
        }
    }
}
