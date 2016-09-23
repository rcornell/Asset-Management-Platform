using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    class DatabaseMessage
    {
        public string Message;
        public bool Success;

        public DatabaseMessage(string message, bool success)
        {
            Message = message;
            Success = success;
        }
    }
}
