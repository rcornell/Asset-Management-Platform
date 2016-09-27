using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using GalaSoft.MvvmLight.Messaging;
using Asset_Management_Platform.Messages;


namespace Asset_Management_Platform.Utility
{
    /// <summary>
    /// This class is designed to communicate with an SQL database 
    /// containing basic data for all the positions covered by
    /// this application.
    /// </summary>
    public class StockDataService : IDisposable
    {
        List<Security> securityList;
        SecurityTableSeederDataService seeder;
        YahooAPIService yahooAPI;

        //public SqlCommand commander;
        //Going to add a branch
        public StockDataService()
        {
            Initialize();
        }

        /// <summary>
        /// Checks the SQL database. If it is null, seeds it.
        /// </summary>
        public void Initialize()
        {
            yahooAPI = new YahooAPIService();
            if (CheckForNullDatabase()) { 
                SeedDatabase();
                Messenger.Default.Send(new DatabaseMessage("Empty database restored.", false));
            }
        }

        /// <summary>
        /// Reads the SQL database and returns a List<Security>
        /// </summary>
        public List<Security> LoadDatabase()
        {
            securityList = new List<Security>();
            using (var connection = new SqlConnection("SQLStorageConnection"))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.CommandText = @"SELECT * FROM STOCKS";
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var cusip = reader.GetString(0);
                        var ticker = reader.GetString(1);
                        var description = reader.GetString(2);
                        var lastPrice = reader.GetFloat(3);
                        var yield = reader.GetDouble(4);
                        securityList.Add(new Security(cusip, ticker, description, lastPrice, yield));
                    }
                }
            }

            return securityList;
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

        public bool UpdateDatabase()
        {

            return true;
        }


        /// <summary>
        /// Insert position(s) into the database and returns true if successful
        /// </summary>
        /// <param name="securitiesToInsert"></param>
        /// <returns></returns>
        public bool InsertIntoDatabase(List<Security> securitiesToInsert)
        {
            return true;
        }

        /// <summary>
        /// Uploads table to database upon closing.
        /// </summary>
        /// <returns></returns>
        public bool UploadDatabase()
        {
            if (securityList != null)
            {
                //Upload to SQL Database
                return true;
            }
            else
                return false;
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
            UploadDatabase();
            securityList = null;
            seeder = null;
        }
    }
}
