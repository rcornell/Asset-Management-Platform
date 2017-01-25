using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Utility
{
    public class YahooAPIResult
    {
        public string Ticker = "";
        public string Description = "";
        public decimal LastPrice;
        public decimal Change;
        public decimal PercentChange;
        public double Yield = 0;
        public double Bid = 0;
        public double Ask = 0;
        public string MarketCap = "0";
        public double PeRatio = 0;
        public int Volume = 0;
        public int BidSize = 0;
        public int AskSize = 0;

        public bool DescriptionIsNA;
        public bool LastPriceIsNA;
        public bool YieldIsNA;
        public bool BidIsNA;
        public bool AskIsNA;
        public bool MarketCapIsNA;
        public bool PeRatioIsNA;
        public bool VolumeIsNA;
        public bool BidSizeIsNA;
        public bool AskSizeIsNA;
        public bool ChangeIsNA;
        public bool PercentChangeIsNA;

        public YahooAPIResult(string result)
        {
            string response = Regex.Replace(result, @"\r\n?|\n", string.Empty);
            string fixedResponse = Regex.Replace(response,@"%", string.Empty);

            if (string.IsNullOrEmpty(fixedResponse.Split(',')[0]))
                Ticker = ""; //Why are you here?
            else
                Ticker = fixedResponse.Split(',')[0].Replace("\"", "");

            LastPriceIsNA = !decimal.TryParse(fixedResponse.Split(',')[1], out LastPrice);
            YieldIsNA = !double.TryParse(fixedResponse.Split(',')[2], out Yield);
            if (fixedResponse.Split(',')[3] == "N/A")
            {
                MarketCapIsNA = true;
                MarketCap = "0.0B";
            }
            else
            {
                MarketCapIsNA = false;
                MarketCap = fixedResponse.Split(',')[3];
            }
            BidIsNA = !double.TryParse(fixedResponse.Split(',')[4], out Bid);
            AskIsNA = !double.TryParse(fixedResponse.Split(',')[5], out Ask);
            PeRatioIsNA = !double.TryParse(fixedResponse.Split(',')[6], out PeRatio);
            VolumeIsNA = !int.TryParse(fixedResponse.Split(',')[7], out Volume);
            BidSizeIsNA = !int.TryParse(fixedResponse.Split(',')[8], out BidSize);
            AskSizeIsNA = !int.TryParse(fixedResponse.Split(',')[9], out AskSize);
            ChangeIsNA = !decimal.TryParse(fixedResponse.Split(',')[10], out Change);
            PercentChangeIsNA = !decimal.TryParse(fixedResponse.Split(',')[11].Replace("\"", ""), out PercentChange);

            //Index out of bounds?
            //Some Descriptions are split by a comma, e.g. ",Inc."
            //So the method searches for an extra item and appends it
            if (string.IsNullOrEmpty(fixedResponse.Split(',')[12]))
                DescriptionIsNA = true;
            else
                Description = fixedResponse.Split(',')[12].Replace("\"", "");
            if (fixedResponse.Split(',').Length == 12)
                Description += fixedResponse.Split(',')[13].Replace("\"", "");
        }

        public YahooAPIResult()
        {

        }
    }
}
