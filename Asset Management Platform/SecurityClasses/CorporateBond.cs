using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.SecurityClasses
{
    class CorporateBond : FixedIncome
    {
        public CorporateBond(string cusip, string ticker, string description, float lastPrice, double yield) 
            : base(cusip, ticker, description, lastPrice, yield)
        {
            SecurityType = "Corporate Bond";
        }

        public CorporateBond(string cusip, string ticker, string description, float lastPrice, double yield, double coupon, string issuer, string rating)
            : base(cusip, ticker, description, lastPrice, yield, coupon, issuer, rating)
        {
            SecurityType = "Corporate Bond";
        }
    }
}
