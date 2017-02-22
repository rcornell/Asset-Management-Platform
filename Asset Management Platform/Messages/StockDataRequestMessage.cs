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

        public StockDataRequestMessage(List<string> tickers)
        {
            Tickers = tickers;
        }

        public StockDataRequestMessage(string ticker)
        {
            Ticker = ticker;
        }
    }
}
