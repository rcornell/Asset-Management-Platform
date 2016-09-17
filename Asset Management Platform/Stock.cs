using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    class Stock : Security
    {

        public double Bid;
        public double Ask;
        public double Beta;
        public double PeRatio;
        public int Volume;
        public int BidSize;
        public int AskSize;

        public Stock(string ticker, string description, double lastPrice)
            : base ( ticker, description, lastPrice)
        {
            Ticker = ticker;
            Description = description;
            LastPrice = lastPrice;
            SecurityType = "Stock";
        }

        public Stock(
            string ticker, 
            string description,  
            double lastPrice,
            double bid,
            double ask,
            double beta,
            double peRatio,
            int volume,
            int bidSize,
            int askSize)
            : base(ticker, description, lastPrice)
        {
            Ticker = ticker;
            Description = description;
            LastPrice = lastPrice;
            Bid = bid;
            Ask = ask;
            Beta = beta;
            PeRatio = peRatio;
            Volume = volume;
            BidSize = bidSize;
            AskSize = askSize;
            SecurityType = "Stock";
        }

    }
}
