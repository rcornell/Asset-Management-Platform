using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Utility
{
    public class YahooAPIResult
    {
        public string Ticker = "";
        public string Description = "";
        public decimal LastPrice;
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


        public YahooAPIResult(string ticker, string description, decimal lastPrice, double yield, 
            double bid, double ask, string marketCap, double peRatio, int volume, int bidSize, int askSize,
            bool descriptionNA, bool lastPriceNA, bool yieldNA, bool bidNA, bool askNA, bool marketCapNA, 
            bool peRatioNA, bool volumeNA, bool bidSizeNA, bool askSizeNA)
        {
            Ticker = ticker;
            Description = description;

            LastPrice = lastPrice;
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
        public YahooAPIResult()
        {

        }
    }
}
