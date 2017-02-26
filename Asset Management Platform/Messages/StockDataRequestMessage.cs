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
        public bool IsStartupRequest;
        public bool IsTradePreviewRequest;
        public bool IsScreenerRequest;

        public StockDataRequestMessage(List<string> tickers, bool isStartup)
        {
            Tickers = tickers;
            IsStartupRequest = isStartup;
        }

        public StockDataRequestMessage(string ticker, bool isStartup, bool isPreview, bool isScreener)
        {
            Ticker = ticker;
            IsStartupRequest = isStartup;
            IsTradePreviewRequest = isPreview;
            IsScreenerRequest = isScreener;
        }

        public StockDataRequestMessage(List<Position> positions, bool isStartup)
        {
            Positions = positions;
            IsStartupRequest = isStartup;
        }

        public StockDataRequestMessage(Position position, bool isStartup)
        {
            Position = position;
            IsStartupRequest = isStartup;
        }
    }
}
