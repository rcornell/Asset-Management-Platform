using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.ViewModel
{
    class Portfolio
    {
        private List<Security> _myPortfolio;
        public List<Security> MyPortfolio
        {
            get { return _myPortfolio; }
            set { _myPortfolio = value; }
        }

        public Portfolio()
        {
            LoadPortfolio();
        }

        private void LoadPortfolio()
        {
            try { }
            catch { }
            //Pull portfolio from Database?
        }


        private void SavePortfolio()
        {
            try { }
            catch { }
        }

        private bool AddToPortfolio(Security securityToAdd)
        {
            return true;
        }

        private bool RemoveFromPortfolio(Security securityToRemove)
        {
            return true;
        }
    }

}
