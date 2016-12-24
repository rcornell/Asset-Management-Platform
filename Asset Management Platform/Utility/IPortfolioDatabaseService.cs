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

        List<Position> GetPositions();

        void LoadPositionsFromDatabase();
        void SavePortfolioToDatabase();
        
        void BackupDatabase();

        void AddToPortfolio(Position positionToAdd);

        void AddToPortfolio(Taxlot taxlotToAdd);
      
        void SellSharesFromPortfolio(Security security, int shares);

        void DeletePortfolio();

    }
}
