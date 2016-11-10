using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Utility
{
    public interface IPortfolioService
    {
        IPortfolio GetPortfolio();

        void _timer_Tick(object sender, EventArgs e);

        void CalculatePositionValues();

        void StartUpdates();

        void StopUpdates();
    }
}
