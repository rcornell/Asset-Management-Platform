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

        Task<Security> GetTradePreviewSecurity(string ticker);

        Task<Security> GetTradePreviewSecurity(string ticker, Security SelectedSecurityType);

        Task<Security> GetSecurityType(string ticker, string tradeType);

        List<Position> GetPositions();

        List<Taxlot> GetTaxlots();

        List<LimitOrder> GetLimitOrders();

        ObservableCollection<PositionByWeight> GetChartAllSecurities();

        ObservableCollection<PositionByWeight> GetChartFundsOnly();

        ObservableCollection<PositionByWeight> GetChartStocksOnly();

        void UpdateTimerInterval(TimeSpan timespan);

        void UpdatePortfolioPrices();

        void UploadPortfolio();

        void DeletePortfolio();
        void UploadAllDatabases();
        void TestLimitOrderMethods();

        Task<bool> BuildLocalPositions(ObservableCollection<Taxlot> taxlots);
    }
}
