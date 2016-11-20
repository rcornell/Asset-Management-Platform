using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public class Taxlot
    {

        private DateTime _datePurchased;
        public DateTime DatePurchased
        {
            get { return _datePurchased; }
            set { _datePurchased = value; }
        }

        private string _ticker;
        public string Ticker
        {
            get { return _ticker; }
            set { _ticker = value; }
        }

        private decimal _costBasis;
        public decimal CostBasis
        {
            get { return _costBasis; }
            set { _costBasis = value; }
        }

        private int _shares;
        public int Shares
        {
            get { return _shares; }
            set { _shares = value; }
        }

        public Taxlot(string ticker, int shares, decimal costBasis, DateTime datePurchased)
        {
            _ticker = ticker;
            _shares = shares;
            _costBasis = costBasis;
            _datePurchased = datePurchased;
        }
    }
}
