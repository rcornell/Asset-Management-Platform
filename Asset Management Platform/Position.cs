using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public class Position
    {
        private string _ticker;
        public string Ticker {
            get { return _ticker;  }
            set { _ticker = value;  }
        }

        private int _sharesOwned;
        public int SharesOwned
        {
            get { return _sharesOwned; }
            set { _sharesOwned = value; }
        }

        public Position(string ticker, int shares)
        {
            _ticker = ticker;
            _sharesOwned = shares;
        }
    }
}
