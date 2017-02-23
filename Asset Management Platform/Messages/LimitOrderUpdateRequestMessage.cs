using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class LimitOrderUpdateRequestMessage
    {
        public List<Security> SecuritiesToCheck;
        public bool IsStartup;

        public LimitOrderUpdateRequestMessage(List<Security> securitiesToCheck, bool isStartup)
        {
            SecuritiesToCheck = securitiesToCheck;
            IsStartup = isStartup;
        }
    }
}
