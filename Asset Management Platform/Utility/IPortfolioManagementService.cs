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

        void GetTradePreviewSecurity(string ticker);

        Task GetSecurityType(string ticker, string tradeType);

        List<Position> GetPositions();

        List<Taxlot> GetTaxlots();

        List<LimitOrder> GetLimitOrders();

        void UpdateTimerInterval(TimeSpan timespan);

        void UpdatePortfolioPrices();

        void DeletePortfolio();
        void TestLimitOrderMethods();

        Task UpdatePortfolioSecuritiesStartup();

        Task<bool> UpdatePortfolioSecuritiesStartupLocal();

        bool IsLocalMode();
    }
}
