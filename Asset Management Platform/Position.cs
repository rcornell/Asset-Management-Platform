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

        public int SharesOwned
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

        private List<Taxlot> _taxlots;
        public List<Taxlot> Taxlots
        {
            get { return _taxlots; } 
            set { _taxlots = value; }
        }

        //public Position(string ticker, int shares)
        //{
        //    _ticker = ticker;
        //    _sharesOwned = shares;
        //}

        //public Position(string ticker, int shares, decimal costBasis)
        //{
        //    _ticker = ticker;
        //    _sharesOwned = shares;
        //    _costBasis = costBasis;
        //}

        public Position(List<Taxlot> taxlots)
        {
            if (_taxlots == null)
                _taxlots = new List<Taxlot>();

            if (taxlots != null)
                _ticker = taxlots[0].Ticker;
        

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
                _ticker = taxlot.Ticker;

            _taxlots.Add(taxlot);
        }

        private string ComputeDatePurchased()
        {
            if (_taxlots.Count > 1)
                return "Multiple";
            else
                return _taxlots[0].DatePurchased.ToShortDateString();
        }

        private int ComputeSharesOwned()
        {
            if (_taxlots.Count == 1)
                return _taxlots[0].Shares;

            int shares = 0;

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

            int shares = 0;

            foreach (var lot in _taxlots)
            {
                shares += lot.Shares;
            }

            var weightedPieces = new List<decimal>();

            foreach (var lot in _taxlots)
            {
                var piece = (lot.Shares / shares) * lot.CostBasis;
                weightedPieces.Add(piece);
            }

            return weightedPieces.Sum();
        }

        private decimal ComputePurchasePrice()
        {
            if (_taxlots.Count == 1)
                return (_taxlots[0].CostBasis / _taxlots[0].Shares);

            int shares = 0;

            foreach (var lot in _taxlots)
            {
                shares += lot.Shares;
            }

            var weightedPieces = new List<decimal>();

            foreach (var lot in _taxlots)
            {
                var piece = (lot.Shares / shares) * lot.PurchasePrice;
                weightedPieces.Add(piece);
            }

            return weightedPieces.Sum();
        }

        public void AddTaxlot(Taxlot taxlotToAdd)
        {
            throw new NotImplementedException();
        }

        public void SellShares(int shares)
        {
            throw new NotImplementedException();
        }
    }
}
