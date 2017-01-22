using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Utility
{
    public interface IPortfolioDatabaseService
    {
        bool CheckDBForPositions();

        List<Taxlot> GetTaxlotsFromDatabase();

        List<Position> GetPositionsFromTaxlots(List<Taxlot> taxlots);

        void SavePortfolioToDatabase();

        void UploadLimitOrdersToDatabase(List<LimitOrder> limitOrders);

        List<LimitOrder> LoadLimitOrdersFromDatabase();

        void BackupDatabase();

        void AddToPortfolioDatabase(Position positionToAdd);

        void AddToPortfolioDatabase(Taxlot taxlotToAdd);
      
        void SellSharesFromPortfolioDatabase(Security security, decimal shares);

        void DeletePortfolio(List<Position> positions);

    }
}
