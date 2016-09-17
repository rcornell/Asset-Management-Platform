using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.ViewModel
{
    class StockList
    {

        private List<Stock> stocks;

        public StockList()
        {
            stocks = GenerateStockList();
        }

        public List<Stock> GenerateStockList()
        {
            var list = new List<Stock>();

            //DoStuff
            //With StockDataService Probably

            return list;
        }

        public void AddStockToList(string name,
            string ticker,
            double lastPrice,
            double bid,
            double ask,
            double beta,
            double peRatio,
            int volume,
            int bidSize,
            int askSize)
        {
            stocks.Add(new Stock(name, ticker, lastPrice, bid, ask, beta, peRatio, volume, bidSize, askSize));
        }

        public void RemoveStockFromList(Stock stockToRemove)
        {
            stocks.Remove(stockToRemove);
        }



    }
}
