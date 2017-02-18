using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.SecurityClasses
{
    public class MunicipalBond : FixedIncome
    {
        public MunicipalBond(string cusip, string ticker, string description, decimal lastPrice, double yield) 
            : base(cusip, ticker, description, lastPrice, yield)
        {
            SecurityType = "Municipal Bond";
        }

        public MunicipalBond(string cusip, string ticker, string description, decimal lastPrice, double yield, double coupon, string issuer, string rating)
            : base(cusip, ticker, description, lastPrice, yield, coupon, issuer, rating)
        {
            SecurityType = "Municipal Bond";
        }
    }
}
