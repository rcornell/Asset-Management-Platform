using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public class DisplayStock
    {
        private Position _position;
        private Stock _stock;

        public string Ticker {
            get {
                return _position.Ticker;
            }
        }

        public string Description
        {
            get
            {
                return _stock.Description;
            }
        }

        public float Price
        {
            get
            {
                return _stock.LastPrice;
            }
        }

        public float MarketValue
        {
            get
            {
                return _stock.LastPrice * _position.SharesOwned;
            }
        }

        public double Bid
        {
            get
            {
                return _stock.Bid;
            }
        }

        public double Ask
        {
            get
            {
                return _stock.Ask;
            }
        }

        public double MarketCap
        {
            get
            {
                return _stock.MarketCap;
            }
        }

        public double PeRatio
        {
            get
            {
                return _stock.PeRatio;
            }
        }

        public double AskSize
        {
            get
            {
                return _stock.AskSize;
            }
        }

        public double BidSize
        {
            get
            {
                return _stock.BidSize;
            }
        }


        public DisplayStock(Position position, Stock stock)
        {
            _position = position;
            _stock = stock;
        }

    }
}
