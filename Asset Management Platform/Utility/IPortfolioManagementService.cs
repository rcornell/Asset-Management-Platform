using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Utility
{
    public interface IPortfolioManagementService
    {
        IPortfolioDatabaseService GetPortfolio();

        void _timer_Tick(object sender, EventArgs e);

        void StartUpdates();

        void StopUpdates();
    }
}
