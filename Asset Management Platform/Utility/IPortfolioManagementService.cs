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

        void AddPosition(Stock stock, string ticker, int shares);

        void SellPosition(Stock stock, string ticker, int shares);

        Stock GetOrderPreviewStock(string ticker);

        List<DisplayStock> GetDisplayStocks();
    }
}
