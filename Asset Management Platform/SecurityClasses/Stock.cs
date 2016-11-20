using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public class Stock : Security
    {

        private double _bid;
        public double Bid
        {
            get { return _bid; }
            set { _bid = value; }
        }

        private double _ask;
        public double Ask
        {
            get { return _ask; }
            set { _ask = value; }
        }

        private double _marketCap;
        public double MarketCap {
            get { return _marketCap; }
            set { _marketCap = value; }
        }

        private double _peRatio;
        public double PeRatio {
            get { return _peRatio; }
            set { _peRatio = value; }
        }

        private int _volume;
        public int Volume {
            get { return _volume; }
            set { _volume = value; }
        }


        private int _bidSize;
        public int BidSize {
            get { return _bidSize; }
            set { _bidSize = value; }
        }

        private int _askSize;
        public int AskSize {
            get { return _askSize; }
            set { _askSize = value; }
        }

        public Stock(string cusip, string ticker, string description, decimal lastPrice, double yield)
            : base (cusip, ticker, description, lastPrice, yield)
        {
            SecurityType = "Stock";
        }

        public Stock(
            string cusip,
            string ticker, 
            string description,  
            decimal lastPrice,
            double yield,
            double bid,
            double ask,
            double marketCap,
            double peRatio,
            int volume,
            int bidSize,
            int askSize)
            : base(cusip, ticker, description, lastPrice, yield)
        {
            Ticker = ticker;
            Description = description;
            LastPrice = lastPrice;
            Bid = bid;
            Ask = ask;
            MarketCap = marketCap;
            PeRatio = peRatio;
            Volume = volume;
            BidSize = bidSize;
            AskSize = askSize;
            SecurityType = "Stock";
        }

    }
}
