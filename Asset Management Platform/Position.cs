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

        private Stock _stock;
        public Stock Stock
        {
            get
            {
                return _stock;
            }
            set
            {
                _stock = value;
            }
        }

        public string Description
        {
            get { return Stock.Description; }
        }

        public decimal Price
        {
            get { return Stock.LastPrice; }
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
            get { return ComputeSharesOwned(); }

        }

        public decimal CostBasis
        {
            get { return ComputeCostBasis(); }
        }

        public decimal PurchasePrice
        {
            get { return ComputePurchasePrice(); }
        }

        public string MarketValue
        {
            get
            {
                var value = Stock.LastPrice * SharesOwned;
                var valueString = value.ToString("#,##0");
                return valueString;
            }
        }

        public string Yield
        {
            get { return string.Format(Stock.Yield + "%"); }
        }

        private List<Taxlot> _taxlots;
        public List<Taxlot> Taxlots
        {
            get { return _taxlots; } 
            set { _taxlots = value; }
        }

        public double Bid
        {
            get { return Stock.Bid; }
        }

        public double Ask
        {
            get { return Stock.Ask; }
        }

        public string MarketCap
        {
            get
            {
                var stringChars = Stock.MarketCap.ToString().Split('.');
                var newString = stringChars[0] + " Billion";
                return newString;
            }
        }

        public double PeRatio
        {
            get { return Stock.PeRatio; }
        }

        public double AskSize
        {
            get { return Stock.AskSize; }
        }

        public double BidSize
        {
            get { return Stock.BidSize; }
        }

        public string Volume
        {
            get { return Stock.Volume.ToString("#,##0"); }
        }

        public string GainLoss
        {
            get { return ((Stock.LastPrice - PurchasePrice) * SharesOwned).ToString("#,##0"); }
        }

        public decimal Change
        {
            get { return Stock.Change; }
        }

        public string PercentChange
        {
            get { return string.Format(@"{0:0.0%}", Stock.PercentChange / 100); }
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

        public Position(Taxlot taxlot)
        {
            if (_taxlots == null)
                _taxlots = new List<Taxlot>();

            if (_taxlots != null)
                Ticker = taxlot.Ticker;

            _taxlots.Add(taxlot);
        }

        private string ComputeDatePurchased()
        {
            if (_taxlots.Count > 1)
                return "Multiple";
            else
                return _taxlots[0].DatePurchased.ToShortDateString();
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
                    var newTaxlot = new Taxlot(lot.Ticker, newShareQuantity, lot.PurchasePrice, lot.DatePurchased, lot.SecurityType);
                    newTaxlots.Add(newTaxlot);
                }
            }
            Taxlots = newTaxlots;
        }

        public Security GetSecurityType()
        {
            return Taxlots[0].SecurityType;
        }
    }
}
