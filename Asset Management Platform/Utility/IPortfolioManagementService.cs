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

        void AddPosition(Security security, string ticker, int shares, string term, double limit, string orderDuration);

        void SellPosition(Security security, string ticker, int shares);

        Security GetOrderPreviewSecurity(string ticker);

        Security GetOrderPreviewSecurity(string ticker, Security SelectedSecurityType);

        List<DisplayStock> GetDisplayStocks();

        List<DisplayMutualFund> GetDisplayMutualFunds();

        void UploadPortfolio();

        void DeletePortfolio();
    }
}
