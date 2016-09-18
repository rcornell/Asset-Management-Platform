using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using System.Data.SqlClient;
using Microsoft.Azure; // Namespace for CloudConfigurationManager 
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types

namespace Asset_Management_Platform.Utility
{
    class StockDataService : IDisposable
    {

        //public SqlCommand commander;
        //Going to add a branch
        public StockDataService()
        {

            if (CheckForNullDatabase())
                SeedDatabase();
            else
                LoadDatabase();

        }

        private void LoadDatabase()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// If Database is empty, return true and seed the database. If not, generate List<Security>
        /// </summary>
        /// <returns></returns>
        private bool CheckForNullDatabase()
        {
            //var mger = new CloudConfigurationManager();
            //select count(**Column_Name**) from table

            //IfDatabaseIsEmpty
            return true;
        }

        private void SeedDatabase()
        {
            throw new NotImplementedException();
        }

        private void DoStuff()
        {
    //        using (var connection = new C.SqlConnection(
    //              "Server = tcp:robcornell.database.windows.net,1433; Initial Catalog = AssetManagementStocks; Persist Security Info = False; User ID = robcornell; Password = Pacman0373; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30;"
    //              ))
    //        {
    //            connection.Open();
    //            Console.WriteLine("Connected successfully.");

    //            Console.WriteLine("Press any key to finish...");
    //            Console.ReadKey(true);
    //        }
        }


        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
