using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        void Buy(Trade trade);

        void Sell(Trade trade);

        Security GetOrderPreviewSecurity(string ticker);

        Security GetOrderPreviewSecurity(string ticker, Security SelectedSecurityType);

        List<DisplayStock> GetDisplayStocks();

        List<DisplayMutualFund> GetDisplayMutualFunds();

        List<LimitOrder> GetLimitOrders();

        ObservableCollection<PositionByWeight> GetChartAllSecurities();

        ObservableCollection<PositionByWeight> GetChartFundsOnly();

        ObservableCollection<PositionByWeight> GetChartStocksOnly();

        void UpdatePortfolioPrices();

        void UploadPortfolio();

        void DeletePortfolio();
        void UploadAllToDatabase();

        Security GetSecurityType(string ticker, string tradeType);

        void TestLimitOrderMethods();
    }
}
