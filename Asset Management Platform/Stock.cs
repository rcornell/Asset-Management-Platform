using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    class Stock
    {
        public string Name;
        public string Ticker;
        public decimal LastPrice;
        public decimal Bid;
        public decimal Ask;
        public decimal Beta;
        public decimal PeRatio;
        public long Volume;
        public long BidSize;
        public long AskSize;

        public Stock() { }

        public Stock(
            string name, 
            string ticker, 
            decimal lastPrice, 
            decimal bid, 
            decimal ask, 
            decimal beta, 
            decimal peRatio, 
            long volume, 
            long bidSize, 
            long askSize)
        {
            Name = name;
            Ticker = ticker;
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
