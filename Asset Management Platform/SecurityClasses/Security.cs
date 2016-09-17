using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public class Security
    {
        public static string SecurityType;

        private string _cusip;
        public string Cusip
        {
            get { return _cusip; }
            set { _cusip = value; }
        }

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

        private double _yield;
        public double Yield
        {
            get { return _yield; }
            set { _yield = value; }
        }

        public Security(string cusip, string ticker, string description, double lastPrice, double yield)
        {
            _cusip = cusip;
            _ticker = ticker;
            _description = description;
            _lastPrice = lastPrice;
            _yield = yield;
        }

    }
}
