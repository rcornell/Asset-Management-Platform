using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Utility
{
    public interface IStockDataService
    {

        void SeedDatabasesIfNeeded();

        List<Security> LoadSecurityDatabase();

        bool IsStockDatabaseNull();

        bool UpdateSecurityDatabase();

        bool InsertIntoDatabase(Security securitiesToInsert);

        void UploadSecuritiesToDatabase();

        void SeedStockDatabase();

        List<Security> GetSecurityList();

        void Dispose();
        List<Security> GetUpdatedPrices();

        Security GetSecurityInfo(string ticker);

        Security GetSecurityInfo(string ticker, Security securityType);

        List<Security> GetSecurityInfo(List<Security> securities);
    }
}
