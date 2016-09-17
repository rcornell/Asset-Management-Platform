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

        private long sharesOwned;
        public long SharesOwned { get { return sharesOwned; } }

        public float Value {
            get { return float.Parse((securityInfo.LastPrice * sharesOwned).ToString()); } }

        public Position(Security security, long shares)
        {
            securityInfo = security;
            sharesOwned = shares;
        }
    }
}
