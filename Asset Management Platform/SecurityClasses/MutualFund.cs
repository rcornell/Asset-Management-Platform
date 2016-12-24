﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    class MutualFund : Security
    {



        public MutualFund(string cusip, string ticker, string description, decimal lastPrice, double yield)
            : base (cusip, ticker, description, lastPrice, yield)
        {
            SecurityType = "Mutual Fund";
        }

        public MutualFund(string cusip, string ticker, string description, double yield, decimal lastPrice, double load)
            : base(cusip, ticker, description, lastPrice, yield)
        {
            SecurityType = "Mutual Fund";
        }
    }
}
