using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Exceptions
{
    public class LimitOrderException : Exception
    {
        public LimitOrderException(string message) : base(message)
        {
        }

        public LimitOrderException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
