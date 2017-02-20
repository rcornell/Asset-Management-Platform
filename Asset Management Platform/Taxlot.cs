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
        private Security _securityType;
        private decimal _shares;
        private decimal _purchasePrice;
        private string _ticker;

        public DateTime DatePurchased
        {
            get { return _datePurchased; }
            set { _datePurchased = value; }
        }
        
        public string Ticker
        {
            get { return _ticker; }
            set { _ticker = value; }
        }

        public decimal CostBasis
        {
            get { return (_purchasePrice * _shares); }
        }
       
        public decimal PurchasePrice
        {
            get { return _purchasePrice; }
            set { _purchasePrice = value; }
        }
        
        public decimal Shares
        {
            get { return _shares; }
            set { _shares = value; }
        }
       
        public Security SecurityType {
            get { return _securityType; }
            set { _securityType = value; }
        }

        public decimal MarketValue
        {
            get { return (Shares * LastPrice); }
        }

        public decimal GainLoss
        {
            get {  return (LastPrice - PurchasePrice) * Shares; }
        }

        public decimal LastPrice { get; set; }

        public Taxlot()
        {
            
        }

        public Taxlot(string ticker, decimal shares, decimal purchasePrice, DateTime datePurchased, Security secType)
        {
            Ticker = ticker;
            Shares = shares;
            PurchasePrice = purchasePrice;
            DatePurchased = datePurchased;
            SecurityType = secType;
        }

        public Taxlot(string ticker, decimal shares, decimal purchasePrice, DateTime datePurchased, Security secType, decimal lastPrice)
        {
            Ticker = ticker;
            Shares = shares;
            PurchasePrice = purchasePrice;
            DatePurchased = datePurchased;
            SecurityType = secType;
            LastPrice = lastPrice;
        }
    }
}
