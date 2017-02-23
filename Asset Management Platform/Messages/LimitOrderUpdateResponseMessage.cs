using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class LimitOrderUpdateResponseMessage
    {
        public List<Security> SecuritiesToCheck;
        public bool IsStartup;

        public LimitOrderUpdateResponseMessage(List<Security> securitiesToCheck, bool isStartup)
        {
            SecuritiesToCheck = securitiesToCheck;
            IsStartup = isStartup;
        }
    }
}
