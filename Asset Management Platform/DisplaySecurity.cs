using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Asset_Management_Platform
{
    public class DisplaySecurity : ObservableObject
    {
        private string _ticker;
        public string Ticker
        {
            get { return _ticker; }
            set
            {
                _ticker = value;
                RaisePropertyChanged(() => Ticker);
            }
        }

        public DisplaySecurity(string ticker)
        {
            Ticker = ticker;
        }
    }
}
