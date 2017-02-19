using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class PortfolioSqlMessage
    {
        public string Message;
        public bool Successful;

        public PortfolioSqlMessage()
        {
            
        }

        public PortfolioSqlMessage(string message, bool successful)
        {
            Message = message;
            Successful = successful;
        }
    }
}
