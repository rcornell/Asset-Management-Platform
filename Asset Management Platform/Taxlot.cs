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

        public decimal CostBasis
        {
            get { return (_purchasePrice * _shares); }
        }

        private decimal _purchasePrice;
        public decimal PurchasePrice
        {
            get { return _purchasePrice; }
            set { _purchasePrice = value; }
        }

        private decimal _shares;
        public decimal Shares
        {
            get { return _shares; }
            set { _shares = value; }
        }

        public Taxlot(string ticker, decimal shares, decimal purchasePrice, DateTime datePurchased)
        {
            Ticker = ticker;
            Shares = shares;
            PurchasePrice = purchasePrice;
            DatePurchased = datePurchased;
        }
    }
}
