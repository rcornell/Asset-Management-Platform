using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    class MutualFund : Security
    {

        private double _yield;
        public double Yield
        {
            get { return _yield; }
            set { _yield = value; }
        }

        private double _load;
        public double Load
        {
            get { return _load; }
            set { _load = value;}
        }


        public MutualFund(string ticker, string description, double lastPrice)
            : base ( ticker, description, lastPrice)
        {
            SecurityType = "Mutual Fund";
        }

        public MutualFund(string ticker, string description, double lastPrice, double yield, double load)
            : base(ticker, description, lastPrice)
        {
            _yield = yield;
            _load = load;
            SecurityType = "Mutual Fund";
        }
    }
}
