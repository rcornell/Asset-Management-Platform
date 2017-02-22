using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class LocalModeMessage
    {
        public bool LocalMode;

        public LocalModeMessage(bool localMode)
        {
            LocalMode = localMode;
        }
    }
}
