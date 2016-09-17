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


        public MutualFund(string cusip, string ticker, string description, float lastPrice, double yield)
            : base (cusip, ticker, description, lastPrice, yield)
        {
            SecurityType = "Mutual Fund";
        }

        public MutualFund(string cusip, string ticker, string description, double yield, float lastPrice, double load)
            : base(cusip, ticker, description, lastPrice, yield)
        {
            _load = load;
            SecurityType = "Mutual Fund";
        }
    }
}
