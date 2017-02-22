using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class SecurityTypeResponseMessage
    {
        public Security Security;
        public string SecurityType;

        public SecurityTypeResponseMessage(Security security)
        {
            Security = security;
            SecurityType = security.SecurityType;
        }
    }
}
