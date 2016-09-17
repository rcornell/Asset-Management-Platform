using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    class Stock : Security
    {
        public decimal Bid;
        public decimal Ask;
        public decimal Beta;
        public decimal PeRatio;
        public long Volume;
        public long BidSize;
        public long AskSize;

        public Stock(string ticker, string description, double lastPrice)
            : base ( ticker, description, lastPrice)
        {
            base.Ticker = ticker;
            base.Description = description;
            base.LastPrice = lastPrice;
        }

        public Stock(
            string ticker, 
            string description,  
            double lastPrice, 
            decimal bid, 
            decimal ask, 
            decimal beta, 
            decimal peRatio, 
            long volume, 
            long bidSize, 
            long askSize)
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
        }

    }
}
