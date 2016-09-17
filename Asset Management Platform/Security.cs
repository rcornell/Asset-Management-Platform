using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public class Security
    {
        private string _ticker;
        public string Ticker
        {
            get { return _ticker; }
            set { _ticker = value; }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private double _lastPrice;
        public double LastPrice
        {
            get { return _lastPrice; }
            set { _lastPrice = value; }
        }

        public Security(string ticker, string description, double lastPrice)
        {
            _ticker = ticker;
            _description = description;
            _lastPrice = lastPrice;
        }

    }
}
