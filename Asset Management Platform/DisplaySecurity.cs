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
        private string _securityType;
        public string SecurityType
        {
            get { return _securityType; }
            set
            {
                _securityType = value;
                RaisePropertyChanged(() => SecurityType);
            }
        }

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

        private bool _hidden;
        public bool Hidden
        {
            get { return _hidden; }
            set
            {
                _hidden = value;
                RaisePropertyChanged(() => Hidden);
            }
        }


        public DisplaySecurity(string ticker, string securityType)
        {
            Hidden = false;
            Ticker = ticker;
        }
    }
}
