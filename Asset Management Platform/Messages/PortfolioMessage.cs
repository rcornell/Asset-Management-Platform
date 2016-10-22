using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    class PortfolioMessage
    {
        public List<Security> Securities;
        public string Message;

        public PortfolioMessage()
        {

        }

        public PortfolioMessage(List<Security> securities)
        {
            Securities = securities;
        }
    }
}
