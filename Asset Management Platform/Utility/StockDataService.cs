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
            if (IsStockDatabaseNull())
            {
                SeedStockDatabase();
                Messenger.Default.Send(new DatabaseMessage("Empty database restored.", false));
            }
            if (IsMutualFundDatabaseNull())
            {
                SeedMutualFundDatabase();
                Messenger.Default.Send(new DatabaseMessage("Empty database restored.", false));
            }
        }

        /// <summary>
        /// Reads the SQL database and returns a List<Security>
        /// </summary>
        public List<Security> LoadSecurityDatabase()
        {
            var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];
            var cmdText = @"SELECT * FROM STOCKS";
            using (var connection = new SqlConnection(storageString))
            {

                using (var command = new SqlCommand(cmdText, connection))
                {
                    string cusip = "";
                    string ticker = "";
                    string description = "";
                    decimal lastPrice = 0;
                    double yield = 0;
                    connection.Open();
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {                       
                        if (!reader.IsDBNull(0))
                            cusip = string.IsNullOrEmpty(reader.GetString(0)) ? "" : reader.GetString(0);
                        if (!reader.IsDBNull(1))
                            ticker = string.IsNullOrEmpty(reader.GetString(1)) ? "" : reader.GetString(1);
                        if (!reader.IsDBNull(2))
                            description = string.IsNullOrEmpty(reader.GetString(2)) ? "" : reader.GetString(2);
                        if (!reader.IsDBNull(3))
                            lastPrice = decimal.Parse(reader.GetString(3)); //if paused here, it's because you're not sure if Float will work.
                        if (!reader.IsDBNull(4))
                            yield = double.Parse(reader.GetString(4));
                        _securityList.Add(new Security(cusip, ticker, description, lastPrice, yield));
                    }
                }
            }

            return _securityList;
        }

        /// <summary>
        /// Check to see if SQL Database is empty. If it is, return true. 
        /// </summary>
        public bool IsStockDatabaseNull()
        {
            var result = 0;
            var cmdText = @"SELECT COUNT(*) from Stocks";
            var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];

            using (var connection = new SqlConnection(storageString))
            {
                connection.Open();
                using (var command = new SqlCommand(cmdText, connection))
                {
                    int.TryParse(command.ExecuteScalar().ToString(), out result);
                }
            }

            if (result > 0)
                return false; //Database IS NOT empty
            else
                return true; //Database IS empty
        }

        public bool IsMutualFundDatabaseNull()
        {
            var result = 0;
            var cmdText = @"SELECT COUNT(*) from MutualFunds";
            var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];

            using (var connection = new SqlConnection(storageString))
            {
                connection.Open();
                using (var command = new SqlCommand(cmdText, connection))
                {
                    int.TryParse(command.ExecuteScalar().ToString(), out result);
                }
            }

            if (result > 0)
                return false; //Database IS NOT empty
            else
                return true; //Database IS empty
        }

        public bool UpdateSecurityDatabase()
        {
            if (_securityList.Count > 0)
            {
                using (var yahooAPI = new YahooAPIService())
                {
                    _securityList = yahooAPI.GetMultipleStocks(_securityList);
                }
            }
            else
                return false; //_securityList has no members

            return true;
        }

        public List<Security> GetUpdatedPrices()
        {

            return new List<Security>();
        }


        /// <summary>
        /// Insert position(s) into the database and returns true if successful
        /// </summary>
        /// <param name="securitiesToInsert"></param>
        /// <returns></returns>
        public bool InsertIntoDatabase(Security securityToInsert)
        {
            if (_securityList.Any(s => s.Ticker == securityToInsert.Ticker))
                return false;
            else
            {
                var insertString = @"INSERT INTO Stocks (Ticker, Description, LastPrice, Yield) VALUES ";
                var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];
                using (var connection = new SqlConnection(storageString))
                {
                    connection.Open(); //is this necessary in a Using?
                    using (var command = new SqlCommand())
                    {
                        var securityInfo = string.Format(@"('{0}', '{1}', {2}, {3});", securityToInsert.Ticker, securityToInsert.Description, 
                                                                                  securityToInsert.LastPrice, securityToInsert.Yield);
                        insertString += securityInfo;
                        command.Connection = connection;
                        command.CommandText = insertString;
                        command.ExecuteNonQuery();
                    }
                }
            }
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
        public void SeedStockDatabase()
        {
            var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];
            using (var seeder = new SecurityTableSeederDataService())
            {
                seeder.LoadStockJsonDataIntoSqlServer(storageString);
            }
        }

        public void SeedMutualFundDatabase()
        {
            var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];
            using (var seeder = new SecurityTableSeederDataService())
            {
                seeder.LoadMutualFundJsonDataIntoSqlServer(storageString);
            }
        }

        public List<Security> GetSecurityList()
        {
            return SecurityList;
        }

        public Stock GetSpecificStockInfo(string ticker)
        {
            if (!string.IsNullOrEmpty(ticker))
            {
                using (var yahooAPI = new YahooAPIService())
                {
                    var result = yahooAPI.GetSingleStock(ticker);
                    var insertedOrNot = InsertIntoDatabase(result);
                    return result;
                }
            }
            return new Stock("", "", "", 0, 0.00); //If you hit this, the ticker was null or empty
        }

        public void Dispose()
        {
            UploadDatabase();
            _securityList = null;
        }
    }
}
