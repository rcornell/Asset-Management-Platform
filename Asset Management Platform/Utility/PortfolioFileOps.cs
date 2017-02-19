using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Asset_Management_Platform.Utility
{
    public class PortfolioFileOps
    {
        public PortfolioFileOps()
        {
            
        }

        public bool TrySaveTaxlots(ObservableCollection<Position> positions)
        {

            return false;
        }
        public bool TrySaveTaxlots(ObservableCollection<Taxlot> taxlots)
        {

            return false;
        }

        public List<Taxlot> TryLoadPortfolio()
        {
            var taxlotsToReturn = new List<Taxlot>();



            return taxlotsToReturn;
        }
    }
}
