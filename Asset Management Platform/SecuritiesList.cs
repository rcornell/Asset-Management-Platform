using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.ViewModel
{
    class SecuritiesList
    {
        //Maintains the list of all known securities in the database.

        private List<Security> _allSecurities;

        public SecuritiesList()
        {
            _allSecurities = GenerateHoldingsList();
        }

        public List<Security> GenerateHoldingsList()
        {
            var list = new List<Security>();

            //DoStuff
            //With StockDataService Probably

            return list;
        }

        public void AddStockToList(string ticker,
            string description,
            double lastPrice,
            double yield,
            double bid,
            double ask,
            double beta,
            double peRatio,
            int volume,
            int bidSize,
            int askSize)
        {
            _allSecurities.Add(new Stock(ticker, description, lastPrice, yield, bid, ask, beta, peRatio, volume, bidSize, askSize));
        }

        public void RemoveStockFromList(Stock stockToRemove)
        {
            _allSecurities.Remove(stockToRemove);
        }



    }
}
