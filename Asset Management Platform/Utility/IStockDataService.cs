using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Utility
{
    public interface IStockDataService
    {

        void TryDatabaseInsert(Security securitiesToInsert);

        void UploadSecuritiesToDatabase();

        List<Security> GetSecurityList();

        Task GetSecurityInfo(string ticker);

        Task GetSecurityInfo(List<string> tickers);

        Task GetUpdatedPricing(List<Security> securities);

        Task GetUpdatedPricing(List<Position> positions);

        List<Security> GetMutualFundExtraData(List<Security> rawSecurities);
    }
}
