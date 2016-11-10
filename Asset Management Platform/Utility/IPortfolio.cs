using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Utility
{
    public interface IPortfolio
    {
        bool CheckDBForPositions();

        List<Position> GetPositions();

        void LoadPortfolioFromDatabase();
        void SavePortfolioToDatabase();
        
        void BackupDatabase();

        void AddToPortfolio(Security securityToAdd, int shares);
      
        void SellSharesFromPortfolio(Security security, int shares);
    }
}
