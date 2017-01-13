using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public class DisplayStock : DisplaySecurity
    {
        private Position _position;
        private Stock _stock;

        public string Ticker {
            get {
                return _position.Ticker;
            }
        }

        public int Shares
        {
            get { return _position.SharesOwned; }
        }

        public string Description
        {
            get
            {
                return _stock.Description;
            }
        }

        public decimal Price
        {
            get
            {
                return _stock.LastPrice;
            }
        }

        public string MarketValue
        {
            get
            {
                var value =_stock.LastPrice * _position.SharesOwned;
                var valueString = value.ToString("#,##0");
                return valueString;
            }
        }

        public string Yield
        {
            get {
                return string.Format(_stock.Yield + "%");
            }
        }

        public double Bid
        {
            get
            {
                return _stock.Bid;
            }
        }

        public double Ask
        {
            get
            {
                return _stock.Ask;
            }
        }

        public string MarketCap
        {
            get
            {
                var stringChars = _stock.MarketCap.ToString().Split('.');
                var newString = stringChars[0] + " Billion";
                return newString;
            }
        }

        public double PeRatio
        {
            get
            {
                return _stock.PeRatio;
            }
        }

        public double AskSize
        {
            get
            {
                return _stock.AskSize;
            }
        }

        public double BidSize
        {
            get
            {
                return _stock.BidSize;
            }
        }

        public string Volume
        {
            get {
                return _stock.Volume.ToString("#,##0");
            }
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
            get {  return (((_stock.LastPrice - _position.PurchasePrice) * _position.SharesOwned)).ToString("#,##0"); }
        }

        public DisplayStock(Position position, Stock stock)
        {
            _position = position;
            _stock = stock;
        }

        public void ReduceShares(int shares)
        {
            _position.SellShares(shares);
        }

        public void AddShares(Taxlot taxlot)
        {
            _position.AddTaxlot(taxlot);
        }

    }
}
