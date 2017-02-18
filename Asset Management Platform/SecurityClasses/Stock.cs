using Asset_Management_Platform.Utility;

namespace Asset_Management_Platform.SecurityClasses
{
    public class Stock : Security
    {

        private double _bid;
        private int _askSize;
        private int _bidSize;
        private int _volume;
        private double _peRatio;
        private double _marketCap;
        private double _ask;

        public double Bid
        {
            get { return _bid; }
            set { _bid = value; }
        }
       
        public double Ask
        {
            get { return _ask; }
            set { _ask = value; }
        }
       
        public double MarketCap {
            get { return _marketCap; }
            set { _marketCap = value; }
        }
        
        public double PeRatio {
            get { return _peRatio; }
            set { _peRatio = value; }
        }
        
        public int Volume {
            get { return _volume; }
            set { _volume = value; }
        }

        public int BidSize {
            get { return _bidSize; }
            set { _bidSize = value; }
        }
       
        public int AskSize {
            get { return _askSize; }
            set { _askSize = value; }
        }

        public Stock(string cusip, string ticker, string description, decimal lastPrice, double yield)
            : base (cusip, ticker, description, lastPrice, yield)
        {
            SecurityType = "Stock";
        }

        public Stock(YahooAPIResult yahooResult)
            : base("", yahooResult.Ticker, yahooResult.Description, yahooResult.LastPrice, yahooResult.Yield)
        {
            SecurityType = "Stock";

            Bid = yahooResult.Bid;
            Ask = yahooResult.Ask;
            MarketCap = double.Parse(yahooResult.MarketCap);
            PeRatio = yahooResult.PeRatio;
            Volume = yahooResult.Volume;
            BidSize = yahooResult.BidSize;
            AskSize = yahooResult.AskSize;
            Change = yahooResult.Change;
            PercentChange = yahooResult.PercentChange;
        }

        public Stock(MarkitJsonResult result)
            : base("", result.Ticker, result.Description, result.LastPrice, 0)
        {
            SecurityType = "Stock";

            Bid = 0;
            Ask = 0;
            MarketCap = double.Parse(result.Marketcap.ToString());
            PeRatio = 0;
            Volume = int.Parse(result.Volume.ToString());
            BidSize = 0;
            AskSize = 0;
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

        public Stock()
        {

        }

        public void UpdateData(YahooAPIResult updatedInfo)
        {
            LastPrice = updatedInfo.LastPrice;
            PeRatio = updatedInfo.PeRatio;
            Yield = updatedInfo.Yield;
            Volume = updatedInfo.Volume;
            Bid = updatedInfo.Bid;
            Ask = updatedInfo.Ask;
            AskSize = updatedInfo.AskSize;
            BidSize = updatedInfo.BidSize;
            MarketCap = double.Parse(updatedInfo.MarketCap);
            Description = updatedInfo.Description;
            Change = updatedInfo.Change;
            PercentChange = updatedInfo.PercentChange;
        }

        public override string ToString()
        {
            return "Stock";
        }

    }
}
