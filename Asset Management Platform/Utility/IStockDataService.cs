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

        bool IsDatabaseNull();

        bool UpdateSecurityDatabase();

        bool InsertIntoDatabase(List<Security> securitiesToInsert);

        void UploadDatabase();

        void SeedDatabase();

        List<Security> GetSecurityList();

        void Dispose();
        List<Security> GetUpdatedPrices();
    }
}
