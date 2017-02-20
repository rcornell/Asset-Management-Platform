using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Utility
{
    public interface IPortfolioDatabaseService
    {

        Task<List<Taxlot>> GetTaxlotsFromDatabase();

        List<Position> GetPositionsFromTaxlots(List<Security> portfolioSecurities);

        List<Position> GetEmptyPositionsList();

        void SavePortfolioToDatabase();

        void UploadLimitOrdersToDatabase(List<LimitOrder> limitOrders);

        List<LimitOrder> LoadLimitOrdersFromDatabase();

        void BackupDatabase();

        void AddToPortfolioDatabase(Taxlot taxlotToAdd);

        void DeletePortfolio(List<Position> positions);

        Task<bool> BuildLocalTaxlots(List<Taxlot> taxlots);

        bool IsLocalMode();

    }
}
