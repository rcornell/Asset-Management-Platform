using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using GalaSoft.MvvmLight.Messaging;
using Asset_Management_Platform.Messages;
using System.Linq;


namespace Asset_Management_Platform.Utility
{
    /// <summary>
    /// This class is designed to communicate with an SQL database 
    /// containing basic data for all the positions covered by
    /// this application.
    /// </summary>
    public class StockDataService : IStockDataService, IDisposable
    {
        private List<Security> _securityList;
        public List<Security> SecurityList {
            get { return _securityList; }
            set { _securityList = value; }
        }


        public StockDataService()
        {
            _securityList = new List<Security>();
        }

        /// <summary>
        /// Checks the SQL database. If it is null, seeds it.
        /// </summary>
        public void Initialize()
        {
            if (CheckForNullDatabase())
            {
                SeedDatabase();
                Messenger.Default.Send(new DatabaseMessage("Empty database restored.", false));
            }
        }

        /// <summary>
        /// Reads the SQL database and returns a List<Security>
        /// </summary>
        public List<Security> LoadSecurityDatabase()
        {
            var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];
            using (var connection = new SqlConnection(storageString))
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
                        _securityList.Add(new Security(cusip, ticker, description, lastPrice, yield));
                    }
                }
            }

            return _securityList;
        }

        /// <summary>
        /// Check to see if SQL Database is empty. If it is, return true. 
        /// </summary>
        public bool CheckForNullDatabase()
        {
            int result = 0;
            var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];

            using (var connection = new SqlConnection(storageString))
            {               
                connection.Open();
                var command = new SqlCommand();
                command.CommandText = @"SELECT COUNT(*) FROM [Stocks];";
                command.Connection = connection;
                var sqlReader = command.ExecuteScalar();
                int countResult;
                int.TryParse(sqlReader.ToString(), out countResult);

                //while (sqlReader.Read())
                //{
                //    sqlReader.
                //}
                //sqlReader.Read
                //result = int.Parse(command.ExecuteReader().ToString());
                //result = int.Parse(command.BeginExecuteReader().ToString());
            }

            if (result > 0)
                return false; //Database IS NOT empty
            else
                return true; //Database IS empty
        }

        public bool UpdateSecurityDatabase()
        {
            var tickers = new List<string>();
            foreach (var security in _securityList)
            {
                tickers.Add(security.Ticker);
            }

            if (tickers.Count > 0)
            {
                using (var yahooAPI = new YahooAPIService())
                {
                    _securityList = yahooAPI.GetData(tickers);
                }
            }
            else
                return false;

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
        public void UploadDatabase()
        {
            if (_securityList != null)
            {
                var insertString = @"INSERT INTO Stocks (Cusip, Ticker, Description, LastPrice, Yield) VALUES ";
                var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];
                using (var connection = new SqlConnection(storageString))
                {
                    using (var command = new SqlCommand())
                    {
                        command.CommandText = @"DELETE * FROM STOCKS;";
                        command.ExecuteNonQuery();

                        var final = _securityList.Last();
                        foreach (var sec in _securityList)
                        {
                            var newValue = string.Format("({0}, {1}, {2}, {3}, {4}) ", sec.Cusip, sec.Ticker, sec.Description, sec.LastPrice, sec.Yield);
                            insertString += newValue;
                            if (sec == final)
                                insertString += @";";
                        }

                        command.ExecuteNonQuery();
                    }
                }
            }
        }


        /// <summary>
        /// Seeds the SQL table if it has no contents using 
        /// local SeedTicker.json file.
        /// </summary>
        public void SeedDatabase()
        {
            var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];
            using (var seeder = new SecurityTableSeederDataService())
            {
                seeder.LoadCsvDataIntoSqlServer(storageString);
            }
        }


        public void Dispose()
        {
            UploadDatabase();
            _securityList = null;
        }
    }
}
