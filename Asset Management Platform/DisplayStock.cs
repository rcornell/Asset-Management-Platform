using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asset_Management_Platform.SecurityClasses;

namespace Asset_Management_Platform
{
    public class DisplayStock : DisplaySecurity
    {
        private Position _position;

        public Position Position
        {
            get { return _position; }
        }
        public Stock Stock { get; set; }

        public decimal Shares
        {
            get { return _position.SharesOwned; }
        }

        public string Description
        {
            get { return Stock.Description; }
        }

        public decimal Price
        {
            get { return Stock.LastPrice; }
        }

        public string MarketValue
        {
            get
            {
                var value = Stock.LastPrice * _position.SharesOwned;
                var valueString = value.ToString("#,##0");
                return valueString;
            }
        }

        public string Yield
        {
            get { return string.Format(Stock.Yield + "%"); }
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

        public string CostBasis
        {
            get { return _position.CostBasis.ToString("#,##0"); }
        }

        public decimal PurchasePrice
        {
            get { return _position.PurchasePrice; }
        }

        public string GainLoss
        {
            get { return ((Stock.LastPrice - _position.PurchasePrice) * _position.SharesOwned).ToString("#,##0"); }
        }

        public decimal Change
        {
            get { return Stock.Change; }
        }

        public string PercentChange
        {
            get { return string.Format(@"{0:0.0%}", Stock.PercentChange / 100); }
        }

        public string DatePurchased
        {
            get { return _position.DatePurchased; }
        }

        public DisplayStock(Position position, Stock stock)
            : base(position.Ticker, stock.SecurityType)
        {
            _position = position;
            Stock = stock;
        }

        public void ReduceShares(decimal shares)
        {
            _position.SellShares(shares);
        }

        public void AddShares(Taxlot taxlot)
        {
            _position.AddTaxlot(taxlot);
        }

    }
}
