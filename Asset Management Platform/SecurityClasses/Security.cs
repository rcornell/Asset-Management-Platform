using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Asset_Management_Platform
{
    public class Security : ObservableObject
    {
        public string SecurityType;

        private string _cusip;
        private string _ticker;
        private string _description;
        private decimal _lastPrice;
        private double _yield;
        private decimal _percentChange;
        private decimal _change;

        public string Cusip
        {
            get { return _cusip; }
            set { _cusip = value;
                RaisePropertyChanged(() => Cusip);
            }
        }       
        public string Ticker
        {
            get { return _ticker; }
            set { _ticker = value;
                RaisePropertyChanged(() => Ticker);
            }
        }        
        public string Description
        {
            get { return _description; }
            set { _description = value;
                RaisePropertyChanged(() => Description);
            }
        }        
        public decimal LastPrice
        {
            get { return _lastPrice; }
            set { _lastPrice = value;
                RaisePropertyChanged(() => LastPrice);
            }
        }        
        public double Yield
        {
            get { return _yield; }
            set { _yield = value;
                RaisePropertyChanged(() => Yield);
            }
        }
        public decimal Change
        {
            get { return _change; }
            set { _change = value; }
        }       
        public decimal PercentChange
        {
            get { return _percentChange; }
            set { _percentChange = value; }
        }

        public Security(string cusip, string ticker, string description, decimal lastPrice, double yield)
        {
            _cusip = cusip;
            _ticker = ticker;
            _description = description;
            _lastPrice = lastPrice;
            _yield = yield;
        }

        public Security()
        {

        }

        public override string ToString()
        {
            return "Security";
        }

    }
}
