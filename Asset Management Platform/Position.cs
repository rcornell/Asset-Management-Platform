using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    class Position
    {
        private Stock stockInfo;
        public Stock StockInfo { get { return stockInfo; } }

        private long sharesOwned;
        public long SharesOwned { get { return sharesOwned; } }

        //public float Value {
         //   get { return float.Parse((stockInfo.LastPrice * sharesOwned).ToString); } }

        public Position(Stock stock, long shares)
        {

        }
    }
}
