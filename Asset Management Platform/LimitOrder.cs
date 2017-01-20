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
        public string TradeType {
            get { return _tradeType; }
            set { _tradeType = value;
                RaisePropertyChanged(() => TradeType);
            }            
        }

        private string _ticker;
        public string Ticker
        {
            get { return _ticker; }
            set { _ticker = value;
                RaisePropertyChanged(() => Ticker);
            }
        }

        private decimal _shares;
        public decimal Shares {
            get { return _shares; }
            set { _shares = value;
                RaisePropertyChanged(() => Shares);
            }
        }


        private decimal _limit;
        public decimal Limit
        {
            get { return _limit; }
            set { _limit = value;
                RaisePropertyChanged(() => Limit);
            }
        }

        private Security _securityType;
        public Security SecurityType {
            get { return _securityType; }
            set { _securityType = value;
                RaisePropertyChanged(() => SecurityType);
            }
        }


        private string _orderDuration;
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
            return string.Format(@"{0} {1} shares of {2} at LIMIT {3}.", TradeType, Shares, Ticker, Limit);
        }

        public bool IsLimitOrderActive(decimal lastPrice)
        {
            if (TradeType == "Buy" && lastPrice <= Limit)
                return true;
            else if (TradeType == "Buy" && lastPrice >= Limit)
                return false;
            else if (TradeType == "Sell" && lastPrice >= Limit)
                return true;
            else if (TradeType == "Sell" && lastPrice <= Limit)
                return false;

            return false; //Why did you reach this?
        }


    }
}
