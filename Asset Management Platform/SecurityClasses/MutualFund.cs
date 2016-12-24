using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    class MutualFund : Security
    {

        private string _assetClass;
        public string AssetClass
        {
            get { return _assetClass; }
            set
            {
                _assetClass = value;
                RaisePropertyChanged(() => AssetClass);
            }
        }

        private string _category;
        public string Category
        {
            get { return _category; }
            set
            {
                _category = value;
                RaisePropertyChanged(() => Category);
            }
        }

        private string _subcategory;
        public string Subcategory {
            get { return _subcategory; }
            set { _subcategory = value;
                RaisePropertyChanged(() => Subcategory);
            }
        }


        public MutualFund(string cusip, string ticker, string description, decimal lastPrice, double yield)
            : base (cusip, ticker, description, lastPrice, yield)
        {
            SecurityType = "Mutual Fund";
        }

        public MutualFund(string cusip, string ticker, string description, double yield, decimal lastPrice, double load)
            : base(cusip, ticker, description, lastPrice, yield)
        {
            SecurityType = "Mutual Fund";
        }
    }
}
