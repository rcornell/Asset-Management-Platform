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

        public string DatePurchased
        {
            get { return ComputeDatePurchased(); }
        }

        private int _sharesOwned;
        public int SharesOwned
        {
            get { return _sharesOwned; }
            set { _sharesOwned = value; }
        }


        private decimal _costBasis;
        public decimal CostBasis
        {
            get { return _costBasis; }
            set { _costBasis = value; }
        }

        private List<Taxlot> _taxlots;
        public List<Taxlot> Taxlots
        {
            get { return _taxlots; } 
            set { _taxlots = value; }
        }

        public Position(string ticker, int shares)
        {
            _ticker = ticker;
            _sharesOwned = shares;
        }

        public Position(string ticker, int shares, decimal costBasis)
        {
            _ticker = ticker;
            _sharesOwned = shares;
            _costBasis = costBasis;
        }

        public Position(List<Taxlot> taxlots)
        {
            if (taxlots != null)
                _ticker = taxlots[0].Ticker; ;
            foreach (var lot in taxlots)
            {
                _taxlots.Add(lot);
            }
        }

        private string ComputeDatePurchased()
        {
            if (_taxlots.Count > 1)
                return "Multiple";
            else
                return _taxlots[0].DatePurchased.ToShortDateString();
        }
    }
}
