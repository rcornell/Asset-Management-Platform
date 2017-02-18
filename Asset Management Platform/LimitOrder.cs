using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Asset_Management_Platform
{
    public class LimitOrder : ObservableObject
    {
        private string _tradeType;
        private string _orderDuration;
        private Security _securityType;
        private decimal _limit;
        private decimal _shares;
        private string _ticker;

        public string TradeType {
            get { return _tradeType; }
            set { _tradeType = value;
                RaisePropertyChanged(() => TradeType);
            }            
        }
       
        public string Ticker
        {
            get { return _ticker; }
            set { _ticker = value;
                RaisePropertyChanged(() => Ticker);
            }
        }
       
        public decimal Shares {
            get { return _shares; }
            set { _shares = value;
                RaisePropertyChanged(() => Shares);
            }
        }
        
        public decimal Limit
        {
            get { return _limit; }
            set { _limit = value;
                RaisePropertyChanged(() => Limit);
            }
        }
       
        public Security SecurityType {
            get { return _securityType; }
            set { _securityType = value;
                RaisePropertyChanged(() => SecurityType);
            }
        }
      
        public string OrderDuration
        {
            get { return _orderDuration; }
            set { _orderDuration = value;
                RaisePropertyChanged(() => OrderDuration);
            }
        }

        public LimitOrder()
        {

        }

        public LimitOrder(Trade trade)
        {
            TradeType = trade.BuyOrSell;
            Ticker = trade.Ticker;
            Shares = trade.Shares;
            Limit = trade.Limit;
            SecurityType = trade.Security;
            OrderDuration = trade.OrderDuration;
        }

        public override string ToString()
        {
            return $@"{TradeType} {Shares} shares of {Ticker} at LIMIT {Limit}.";
        }

        public bool IsLimitOrderActive(decimal lastPrice)
        {
            if (TradeType == "Buy" && lastPrice <= Limit)
                return true;
            if (TradeType == "Buy" && lastPrice >= Limit)
                return false;
            if (TradeType == "Sell" && lastPrice >= Limit)
                return true;
            if (TradeType == "Sell" && lastPrice <= Limit)
                return false;

            return false; //Why did you reach this?
        }
    }
}
