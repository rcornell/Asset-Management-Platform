using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class TradeMessage
    {
        public string Message;

        public string Ticker;

        public decimal Shares;

        public TradeMessage()
        {

        }

        public TradeMessage(string ticker, int shares)
        {
            Ticker = ticker;
            Shares = shares;
            Message = string.Format(@"There is a problem with your order to buy {0} shares of {1}. Please make sure the terms are correct and that your selected security type (Stock or Mutual Fund) is correct.", shares, ticker);
        }

        public TradeMessage(string ticker, int shares, string message)
        {
            Ticker = ticker;
            Shares = shares;
            Message = message;
        }
    }
}
