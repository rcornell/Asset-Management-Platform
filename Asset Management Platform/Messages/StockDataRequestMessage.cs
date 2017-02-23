using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class StockDataRequestMessage
    {
        public List<string> Tickers;
        public List<Position> Positions;
        public Position Position;
        public string Ticker;
        public bool IsStartup;
        public bool IsTradePreview;

        public StockDataRequestMessage(List<string> tickers, bool isStartup)
        {
            Tickers = tickers;
            IsStartup = isStartup;
        }

        public StockDataRequestMessage(string ticker, bool isStartup, bool isPreview)
        {
            Ticker = ticker;
            IsStartup = isStartup;
            IsTradePreview = isPreview;
        }

        public StockDataRequestMessage(List<Position> positions, bool isStartup)
        {
            Positions = positions;
            IsStartup = isStartup;
        }

        public StockDataRequestMessage(Position position, bool isStartup)
        {
            Position = position;
            IsStartup = isStartup;
        }
    }
}
