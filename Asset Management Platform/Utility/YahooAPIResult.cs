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


        public YahooAPIResult(string ticker, string description, decimal lastPrice, decimal change, 
            decimal percentChange, double yield, double bid, double ask, string marketCap, double peRatio, 
            int volume, int bidSize, int askSize, bool descriptionNA, bool lastPriceNA, bool yieldNA, 
            bool bidNA, bool askNA, bool marketCapNA, bool peRatioNA, bool volumeNA, bool bidSizeNA, bool askSizeNA)
        {
            Ticker = ticker;
            Description = description;

            LastPrice = lastPrice;
            Change = change;
            PercentChange = percentChange;
            Yield = yield;
            Bid = bid;
            Ask = ask;
            MarketCap = marketCap;
            PeRatio = peRatio;
            Volume = volume;
            BidSize = bidSize;
            AskSize = askSize;

            DescriptionIsNA = descriptionNA;
            LastPriceIsNA = lastPriceNA;
            YieldIsNA = yieldNA;
            BidIsNA = bidNA;
            AskIsNA = askNA;
            MarketCapIsNA = marketCapNA;
            PeRatioIsNA = peRatioNA;
            VolumeIsNA = volumeNA;
            BidSizeIsNA = bidSizeNA;
            AskSizeIsNA = askSizeNA;
        }

        public YahooAPIResult(string result)
        {
            string fixedResponse = Regex.Replace(result, @"\r\n?|\n", string.Empty);

            bool descriptionIsNA = false;
            bool lastPriceIsNA = false;
            bool yieldIsNA = false;
            bool bidIsNA = false;
            bool askIsNA = false;
            bool marketCapIsNA = false;
            bool peRatioIsNA = false;
            bool volumeIsNA = false;
            bool bidSizeIsNA = false;
            bool askSizeIsNA = false;
            bool changeIsNA = false;
            bool percentChangeIsNA = false;

            if (string.IsNullOrEmpty(fixedResponse.Split(',')[0]))
                Ticker = ""; //Why are you here?
            else
                Ticker = fixedResponse.Split(',')[0].Replace("\"", "");

            lastPriceIsNA = !decimal.TryParse(fixedResponse.Split(',')[1], out LastPrice);
            yieldIsNA = !double.TryParse(fixedResponse.Split(',')[2], out Yield);
            if (fixedResponse.Split(',')[3] == "N/A")
            {
                marketCapIsNA = true;
                MarketCap = "0.0B";
            }
            else
            {
                marketCapIsNA = false;
                MarketCap = fixedResponse.Split(',')[3];
            }
            bidIsNA = !double.TryParse(fixedResponse.Split(',')[4], out Bid);
            askIsNA = !double.TryParse(fixedResponse.Split(',')[5], out Ask);
            peRatioIsNA = !double.TryParse(fixedResponse.Split(',')[6], out PeRatio);
            volumeIsNA = !int.TryParse(fixedResponse.Split(',')[7], out Volume);
            bidSizeIsNA = !int.TryParse(fixedResponse.Split(',')[8], out BidSize);
            askSizeIsNA = !int.TryParse(fixedResponse.Split(',')[9], out AskSize);
            changeIsNA = !decimal.TryParse(fixedResponse.Split(',')[10], out Change);
            percentChangeIsNA = !decimal.TryParse(fixedResponse.Split(',')[11], out PercentChange);

            //Index out of bounds?
            //Some Descriptions are split by a comma, e.g. ",Inc."
            //So the method searches for an extra item and appends it
            if (string.IsNullOrEmpty(fixedResponse.Split(',')[10]))
                descriptionIsNA = true;
            else
                Description = fixedResponse.Split(',')[10].Replace("\"", "");
            if (fixedResponse.Split(',').Length == 12)
                Description += fixedResponse.Split(',')[11].Replace("\"", "");
        }

        public YahooAPIResult()
        {

        }
    }
}
