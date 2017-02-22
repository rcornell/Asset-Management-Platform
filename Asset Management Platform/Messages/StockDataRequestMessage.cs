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
        public string Ticker;
        public bool IsStartup;

        public StockDataRequestMessage(List<string> tickers, bool isStartup)
        {
            Tickers = tickers;
            IsStartup = isStartup;
        }

        public StockDataRequestMessage(string ticker)
        {
            Ticker = ticker;
        }
    }
}
