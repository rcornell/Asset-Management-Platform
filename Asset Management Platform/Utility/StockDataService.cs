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
    /// <summary>
    /// This class is designed to communicate with an SQL database 
    /// containing basic data for all the positions covered by
    /// this application.
    /// </summary>
    class StockDataService : IDisposable
    {
        List<Security> securityList;
        SecurityTableSeederDataService seeder;

        //public SqlCommand commander;
        //Going to add a branch
        public StockDataService()
        {

        }

        /// <summary>
        /// Checks the SQL database. If it is null, seeds it.
        /// If not null, loads into memory.
        /// </summary>
        public void Initialize()
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
        /// Check to see if SQL Database is empty. If it is, return true. 
        /// </summary>
        private bool CheckForNullDatabase()
        {
            int result = 0;

            using (var connection = new SqlConnection("SQLStorageConnection"))
            {               
                connection.Open();
                var command = new SqlCommand();
                command.CommandText = @"SELECT COUNT(*) FROM STOCKS";
                result = int.Parse(command.BeginExecuteReader().ToString());
            }

            if (result > 0)
                return false; //Database IS NOT empty
            else
                return true; //Database IS empty
        }

        /// <summary>
        /// Seeds the SQL table if it has no contents using 
        /// local SeedTicker.json file.
        /// </summary>
        private void SeedDatabase()
        {
            seeder = new SecurityTableSeederDataService();
            seeder.LoadCsvDataIntoSqlServer("StorageConnectionString");
        }


        public void Dispose()
        {
            securityList = null;
            seeder = null;
        }
    }
}
