using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Asset_Management_Platform
{
    public class Position : ObservableObject
    {

        private Security _security;
        public Security Security
        {
            get
            {
                return _security;
            }
            set
            {
                _security = value;
            }
        }

        public string Description
        {
            get { return Security.Description; }
        }

        public decimal Price
        {
            get { return Security.LastPrice; }
        }

        private string _ticker;
        public string Ticker {
            get { return _ticker;  }
            set { _ticker = value;  }
        }

        public string DatePurchased
        {
            get { return ComputeDatePurchased(); }
        }

        public decimal SharesOwned
        {
            get
            {
                return ComputeSharesOwned();                
            }
        }

        public decimal CostBasis
        {
            get
            {
                var cost = ComputeCostBasis();
                return cost;
            }
        }

        public decimal PurchasePrice
        {
            get { return ComputePurchasePrice(); }
        }

        public string MarketValue
        {
            get
            {
                var value = Security.LastPrice * SharesOwned;
                var valueString = value.ToString("#,##0");
                return valueString;
            }
        }

        public string Yield
        {
            get { return string.Format(Security.Yield + "%"); }
        }

        private List<Taxlot> _taxlots;
        public List<Taxlot> Taxlots
        {
            get { return _taxlots; } 
            set { _taxlots = value; }
        }

        public double Bid
        {
            get {
                if (Security is Stock)
                {
                    var s = (Stock)Security;
                    return s.Bid;
                }
                return 0;
            }
        }

        public double Ask
        {
            get
            {
                if (Security is Stock)
                {
                    var s = (Stock)Security;
                    return s.Ask;
                }
                return 0;
            }
        }

        public string MarketCap
        {
            get
            {
                if (Security is Stock)
                {
                    var s = (Stock)Security;
                    var stringChars = s.MarketCap.ToString().Split('.');
                    var newString = stringChars[0] + " Billion";
                    return newString;
                }
                return "0";
            }
        }

        public double PeRatio
        {
            get
            {
                if (Security is Stock)
                {
                    var s = (Stock)Security;
                    return s.PeRatio;
                }
                return 0;
            }
        }

        public double AskSize
        {
            get
            {
                if (Security is Stock)
                {
                    var s = (Stock)Security;
                    return s.AskSize;
                }
                return 0;
            }
        }

        public double BidSize
        {
            get
            {
                if (Security is Stock)
                {
                    var s = (Stock)Security;
                    return s.BidSize;
                }
                return 0;
            }
        }

        public string Volume
        {
            get
            {
                if (Security is Stock)
                {
                    var s = (Stock)Security;
                    return s.Volume.ToString("#,##0");
                }
                return "0";
            }
        }

        public string GainLoss
        {
            get { return ((Security.LastPrice - PurchasePrice) * SharesOwned).ToString("#,##0"); }
        }

        public decimal Change
        {
            get { return Security.Change; }
        }

        public string PercentChange
        {
            get { return string.Format(@"{0:0.0%}", Security.PercentChange / 100); }
        }

        private bool _hidden;
        public bool Hidden
        {
            get { return _hidden; }
            set
            {
                _hidden = value;
                RaisePropertyChanged(() => Hidden);
            }
        }

        public Position(List<Taxlot> taxlots)
        {
            if (_taxlots == null)
                _taxlots = new List<Taxlot>();

            if (taxlots != null)
                Ticker = taxlots[0].Ticker;        

            foreach (var lot in taxlots)
            {
                _taxlots.Add(lot);
            }
        }

        public Position(Taxlot taxlot, Security security)
        {
            if (_taxlots == null)
                _taxlots = new List<Taxlot>();

            if (_taxlots != null)
                Ticker = taxlot.Ticker;

            Taxlots.Add(taxlot);
            Security = security;
            
        }

        private string ComputeDatePurchased()
        {
            if (_taxlots.Count > 1)
                return "Multiple";
            else
                return _taxlots[0].DatePurchased.ToString();
        }

        private decimal ComputeSharesOwned()
        {
            if (_taxlots.Count == 1)
                return _taxlots[0].Shares;

            decimal shares = 0;

            foreach (var lot in _taxlots)
            {
                shares += lot.Shares;
            }

            return shares;
        }

        private decimal ComputeCostBasis()
        {
            if (_taxlots.Count == 1)
                return _taxlots[0].CostBasis;

            decimal totalCost = 0;

            foreach (var lot in Taxlots)
            {
                totalCost += lot.CostBasis;
            }

            return totalCost;
        }

        private decimal ComputePurchasePrice()
        {
            if (_taxlots.Count == 1)
                return _taxlots[0].PurchasePrice;

            decimal averagePrice = 0;
            decimal totalShares = 0;

            foreach (var lot in Taxlots)
            {
                totalShares += lot.Shares;
            }

            foreach (var lot in Taxlots)
            {
                decimal weight = lot.Shares / totalShares;
                averagePrice += lot.PurchasePrice * weight;
            }

            return averagePrice;
        }

        public void AddTaxlot(Taxlot taxlotToAdd)
        {
            Taxlots.Add(taxlotToAdd);
        }

        public void SellShares(decimal sharesToSell)
        {
            var newTaxlots = new List<Taxlot>();

            foreach (var lot in Taxlots)
            {
                if(sharesToSell > lot.Shares)
                {
                    sharesToSell -= lot.Shares;
                    continue;
                }
                else if (sharesToSell == lot.Shares)
                {
                    sharesToSell = 0;
                    continue;
                }
                else if (sharesToSell < lot.Shares)
                {
                    var newShareQuantity = lot.Shares - sharesToSell;
                    sharesToSell = 0;
                    var newTaxlot = new Taxlot(lot.Ticker, newShareQuantity, lot.PurchasePrice, lot.DatePurchased, lot.SecurityType, Price);
                    newTaxlots.Add(newTaxlot);
                }
            }
            Taxlots = newTaxlots;
        }

        public void UpdateTaxlotPrices(decimal lastPrice)
        {
            foreach (var lot in Taxlots)
            {
                lot.LastPrice = lastPrice;
            }
        }

        public Security GetSecurityType()
        {
            return Taxlots[0].SecurityType;
        }
    }
}
