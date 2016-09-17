using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    class Position
    {
        private Security securityInfo;
        public Security SecurityInfo { get { return securityInfo; } }

        private int sharesOwned;
        public int SharesOwned { get { return sharesOwned; } }

        public float Value {
            get { return GetValue(); } }

        public Position(Security security, int shares)
        {
            securityInfo = security;
            sharesOwned = shares;
        }

        private float GetValue()
        {
            float value = 0;

            var type = GetType();
            if (type == typeof(FixedIncome))
            {
                //If the below return doesn't exit the method, refactor this. Otherwise it will recalculate value again when it exits the if.
                value = (sharesOwned * 1000) * securityInfo.LastPrice;
                return value;
            }

            value = sharesOwned * securityInfo.LastPrice;
            return value;
        }
    }
}
