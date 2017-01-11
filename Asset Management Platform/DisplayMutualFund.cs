using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public class DisplayMutualFund
    {

        private Position _position;
        private MutualFund _fund;

        public string Ticker
        {
            get
            {
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
                return _fund.Description;
            }
        }

        public decimal Price
        {
            get
            {
                return _fund.LastPrice;
            }
        }

        public string MarketValue
        {
            get
            {
                var value = _fund.LastPrice * _position.SharesOwned;
                var valueString = value.ToString("#,##0");
                return valueString;
            }
        }

        public string Yield
        {
            get
            {
                return string.Format(_fund.Yield + "%");
            }
        }
        
        public string AssetClass
        {
            get
            {
                return _fund.AssetClass;
            }
        }   

        public string Category
        {
            get
            {
                return _fund.Category;
            }
        }

        public string Subcategory
        {
            get
            {
                return _fund.Subcategory;
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
            get { return (((_fund.LastPrice - _position.PurchasePrice) * _position.SharesOwned)).ToString("#,##0"); }
        }

        public DisplayMutualFund(Position position, MutualFund fund)
        {
            _position = position;
            _fund = fund;
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
