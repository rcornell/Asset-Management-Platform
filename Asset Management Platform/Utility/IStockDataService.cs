using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Utility
{
    public interface IStockDataService
    {

        List<Security> LoadSecurityDatabase();

        bool InsertIntoDatabase(Security securitiesToInsert);

        void UploadSecuritiesToDatabase();

        List<Security> GetSecurityList();

        Security GetSecurityInfo(string ticker);

        void GetUpdatedPricing(List<Security> securities);

        List<Security> GetSecurityInfo(List<string> tickers);

        List<Security> GetMutualFundExtraData(List<Security> rawSecurities);
    }
}
