using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class SecurityDatabaseMessage
    {
        public List<Security> SecurityList;

        public SecurityDatabaseMessage(List<Security> secList)
        {
            SecurityList = secList;
        }
    }
}
