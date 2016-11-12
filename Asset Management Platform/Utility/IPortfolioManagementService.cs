using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Utility
{
    public interface IPortfolioManagementService
    {
        void _timer_Tick(object sender, EventArgs e);

        void StartUpdates();

        void StopUpdates();

        void AddPosition(string ticker, int shares);

        void SellPosition(string ticker, int shares);

        List<DisplayStock> GetDisplayStocks();
    }
}
