using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public class Portfolio
    {
        private List<Position> _myPortfolio;
        public List<Position> MyPortfolio
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

        private bool AddToPortfolio(Security securityToAdd, int shares)
        {
            return true;
        }

        private bool SellSharesFromPortfolio(Security securityToRemove, int shares)
        {
            return true;
        }

        private bool RemoveFromPortfolio(Security securityToRemove)
        {
            return true;
        }
    }

}
