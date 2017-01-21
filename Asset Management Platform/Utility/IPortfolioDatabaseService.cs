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

        List<Position> CreatePositionsFromTaxlots(List<Taxlot> taxlots);

        void SavePortfolioToDatabase();

        void UploadLimitOrdersToDatabase(List<LimitOrder> limitOrders);

        List<LimitOrder> LoadLimitOrdersFromDatabase();

        void BackupDatabase();

        void AddToPortfolio(Position positionToAdd);

        void AddToPortfolio(Taxlot taxlotToAdd);
      
        void SellSharesFromPortfolio(Security security, decimal shares);

        void DeletePortfolio();

    }
}
