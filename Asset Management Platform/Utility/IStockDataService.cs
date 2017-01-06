using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Utility
{
    public interface IStockDataService
    {

        void Initialize();

        List<Security> LoadSecurityDatabase();

        bool IsStockDatabaseNull();

        bool UpdateSecurityDatabase();

        bool InsertIntoDatabase(Security securitiesToInsert);

        void UploadDatabase();

        void SeedStockDatabase();

        List<Security> GetSecurityList();

        void Dispose();
        List<Security> GetUpdatedPrices();

        Stock GetSpecificStockInfo(string ticker);
    }
}
