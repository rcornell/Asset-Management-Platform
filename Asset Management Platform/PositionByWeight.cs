using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Asset_Management_Platform
{
    public class PositionByWeight : ObservableObject
    {
        private string _ticker;
        private decimal _weight;

        public string Ticker
        {
            get { return _ticker; }
            set
            {
                _ticker = value;
                RaisePropertyChanged(() => Ticker);                
            }
        }
       
        public decimal Weight
        {
            get { return _weight; }
            set { _weight = value;
                RaisePropertyChanged(() => Weight);
            }
        }

        public PositionByWeight(string ticker, decimal weight)
        {
            Ticker = ticker;
            Weight = weight;
        }
    }
}
