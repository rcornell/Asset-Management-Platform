using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Utility
{
    public interface IPortfolioDatabaseService
    {
        Task BuildDatabaseTaxlots();

        List<Position> GetPositionsFromTaxlots(List<Security> portfolioSecurities);

        void GetPositionsFromTaxlots();

        List<Position> GetEmptyPositionsList();

        void SavePortfolioToDatabase();

        void UploadLimitOrdersToDatabase();

        Task LoadLimitOrdersFromDatabase();

        void BackupDatabase();

        void DeletePortfolio(List<Position> positions);
    }
}
