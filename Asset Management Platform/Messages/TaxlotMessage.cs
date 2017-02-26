using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Messages
{
    public class TaxlotMessage
    {
        public List<Taxlot> Taxlots;
        public bool IsStartup;
        public bool IsLocalMode;

        public TaxlotMessage(List<Taxlot> taxlots, bool isStartup, bool isLocalMode)
        {
            Taxlots = taxlots;
            IsStartup = isStartup;
            IsLocalMode = isLocalMode;
        }
    }
}
