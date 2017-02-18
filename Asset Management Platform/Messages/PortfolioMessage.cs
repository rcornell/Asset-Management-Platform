using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class PortfolioMessage
    {
        public List<Security> Securities;
        public string Message;
        public Dictionary<string, double> PositionValues;

        public PortfolioMessage()
        {
            PositionValues = new Dictionary<string, double>();
        }

        public PortfolioMessage(List<Security> securities)
        {
            PositionValues = new Dictionary<string, double>();
            Securities = securities;
        }
    }
}
