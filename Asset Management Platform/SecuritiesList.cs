using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.ViewModel
{
    class SecuritiesList
    {
        //Maintains the list of all known securities in the database.

        private List<Security> _allSecurities;

        public SecuritiesList()
        {
            _allSecurities = GenerateHoldingsList();
        }

        public List<Security> GenerateHoldingsList()
        {
            var list = new List<Security>();

            //DoStuff
            //With StockDataService Probably

            return list;
        }

        public bool SaveHoldingsList()
        {
            //Store _allSecurities in database
            return true;
        }

        public void RemoveSecurityFromList(Security securityToRemove)
        {
            if (securityToRemove != null)
                _allSecurities.Remove(securityToRemove);
        }

        public void AddSecurityToList(Security securityToAdd)
        {
            if (securityToAdd != null)
                _allSecurities.Add(securityToAdd);
        }
    }
}
