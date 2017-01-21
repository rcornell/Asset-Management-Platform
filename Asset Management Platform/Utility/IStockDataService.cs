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

        bool InsertIntoDatabase(Security securitiesToInsert);

        void UploadSecuritiesToDatabase();

        void SeedStockDatabase();

        List<Security> GetSecurityList();

        Security GetSecurityInfo(string ticker);

        Security GetSecurityInfo(string ticker, Security securityType);

        List<Security> GetSecurityInfo(List<Security> securities);

        List<Security> GetSecurityInfo(List<string> tickers);

        List<Security> GetMutualFundExtraData(List<Security> rawSecurities);
    }
}
