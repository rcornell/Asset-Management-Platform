using Asset_Management_Platform.Utility;
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
        private string _category;
        private string _subcategory;




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
        public MutualFund(string cusip, string ticker, string description, decimal lastPrice, double yield, string assetClass, string category, string subcategory)
            : base(cusip, ticker, description, lastPrice, yield)
        {
            SecurityType = "Mutual Fund";
            AssetClass = assetClass;
            Category = category;
            Subcategory = subcategory;
        }

        public MutualFund(YahooAPIResult yahooResult)
            : base ("", yahooResult.Ticker, yahooResult.Description, yahooResult.LastPrice, yahooResult.Yield)
        {
            SecurityType = "Mutual Fund";
            AssetClass = "-";
            Category = "-";
            Subcategory = "-";
            Change = yahooResult.Change;
            PercentChange = yahooResult.PercentChange;
        }

        public MutualFund()
        {
            SecurityType = "Mutual Fund";
        }

        public void UpdateData(YahooAPIResult updatedInfo)
        {
            LastPrice = updatedInfo.LastPrice;
            Description = updatedInfo.Description;
            Yield = updatedInfo.Yield;
            Change = updatedInfo.Change;
            PercentChange = updatedInfo.PercentChange;
        }

        public override string ToString()
        {
            return "Mutual Fund";
        }
    }
}
