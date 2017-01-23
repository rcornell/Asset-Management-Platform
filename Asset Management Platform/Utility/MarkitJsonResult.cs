using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Utility
{
    public class MarkitJsonResult
    {
        public string Status;
        public string Ticker;
        public string Description;
        public decimal LastPrice;
        public decimal Change;
        public decimal ChangePercent;
        public string Timestamp;
        public decimal MSDate;
        public decimal Marketcap;
        public decimal Volume;
        public decimal ChangeYTD;
        public decimal ChangePercentYTD;
        public decimal High;
        public decimal Low;
        public decimal Open;

        public MarkitJsonResult()
        {

        }

        public MarkitJsonResult(string status, string name, string symbol, decimal lastPrice, decimal change,
            decimal changePercent, string timestamp, decimal msdate, decimal marketcap, decimal volume,
            decimal changeYTD, decimal changePercentYTD, decimal high, decimal low, decimal open)
        {
            Status = status;
            Description = name;
            Ticker = symbol;
            LastPrice = lastPrice;
            Change = change;
            ChangePercent = changePercent;
            Timestamp = timestamp;
            MSDate = msdate;
            Marketcap = marketcap;
            Volume = volume;
            ChangeYTD = changeYTD;
            ChangePercentYTD = changePercentYTD;
            High = high;
            Low = low;
            Open = open;
        }
    }
}
