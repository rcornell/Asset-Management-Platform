using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class DatabaseMessage
    {
        public string Message;
        public bool PositionsSuccessful;
        public bool LimitOrdersSuccessful;

        public DatabaseMessage()
        {

        }

        public DatabaseMessage(string message, bool positionsSuccessful, bool limitOrdersSuccessful)
        {
            Message = message;
            PositionsSuccessful = positionsSuccessful;
            LimitOrdersSuccessful = limitOrdersSuccessful;
        }
    }
}
