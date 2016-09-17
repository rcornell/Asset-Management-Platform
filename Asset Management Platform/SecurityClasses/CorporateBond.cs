using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.SecurityClasses
{
    class CorporateBond : FixedIncome
    {
        public CorporateBond(string cusip, string ticker, string description, double lastPrice, double yield) 
            : base(cusip, ticker, description, lastPrice, yield)
        {

        }

        public CorporateBond(string cusip, string ticker, string description, double lastPrice, double yield, double coupon, string issuer, string rating)
            : base(cusip, ticker, description, lastPrice, yield, coupon, issuer, rating)
        {
            
        }
    }
}
