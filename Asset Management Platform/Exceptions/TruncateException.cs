using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Exceptions
{
    public class TruncateException : Exception
    {
        public TruncateException(string message) : base(message)
        {
        }

        public TruncateException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
