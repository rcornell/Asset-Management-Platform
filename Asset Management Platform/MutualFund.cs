using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    class MutualFund : Security
    {

        private double _load;
        public double Load
        {
            get { return _load; }
            set { _load = value;}
        }


        public MutualFund(string ticker, string description, double lastPrice, double yield)
            : base ( ticker, description, lastPrice, yield)
        {
            SecurityType = "Mutual Fund";
        }

        public MutualFund(string ticker, string description, double yield, double lastPrice, double load)
            : base(ticker, description, lastPrice, yield)
        {
            _load = load;
            SecurityType = "Mutual Fund";
        }
    }
}
