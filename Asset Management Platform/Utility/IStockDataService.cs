using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Asset_Management_Platform.Messages;

namespace Asset_Management_Platform.Utility
{
    public interface IStockDataService
    {

        void TryDatabaseInsert(Security securitiesToInsert);

        void UploadSecuritiesToDatabase();

        Task GetSecurityInfo(string ticker);

        Task GetSecurityInfo(StockDataRequestMessage message);

        Task GetUpdatedPricing(List<Security> securities);

        Task GetUpdatedPricing(List<Position> positions);

        List<Security> GetMutualFundExtraData(List<Security> rawSecurities);
    }
}
