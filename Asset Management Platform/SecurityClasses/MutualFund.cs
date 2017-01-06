using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public class MutualFund : Security
    {

        private string _assetClass;

        [JsonProperty("AssetClass")]
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
        [JsonProperty("Category")]
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
        [JsonProperty("Subcategory")]
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

        [JsonConstructor]
        public MutualFund(string ticker, string assetClass, string category, string subcategory)
            : base("", ticker, "", 0, 0.00)
        {
            SecurityType = "Mutual Fund";
            AssetClass = assetClass;
            Category = category;
            Subcategory = subcategory;
        }


        //public MutualFund(string cusip, string ticker, string description, double yield, decimal lastPrice, double load)
        //    : base(cusip, ticker, description, lastPrice, yield)
        //{
        //    SecurityType = "Mutual Fund";
        //}
    }
}
