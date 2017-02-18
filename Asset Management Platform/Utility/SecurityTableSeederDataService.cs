using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Asset_Management_Platform.Utility
{
    class SecurityTableSeederDataService : IDisposable
    {
        private List<SecurityClasses.StockFromJSON> stockList;
        private List<MutualFund> mutualFundList;
        protected const string _truncateStockLiveTableCommandText = @"TRUNCATE TABLE [Stocks]";
        protected const string _truncateMutualFundLiveTableCommandText = @"TRUNCATE TABLE [MutualFunds]";
        protected const int _batchSize = 2000; //max number times this look to add. Adjust for need vs. speed.

        public SecurityTableSeederDataService()
        {
            
        }

        public void LoadMutualFundJsonDataIntoSqlServer(string connectionString)
        {
            //Can I find a better way that deserializing to the StockTicker class with one property?
            var fileInfo = new FileInfo(@"SeedJson\SeedMutualFundInfo.json");
            var tickerJson = File.ReadAllText(fileInfo.FullName);
            mutualFundList = JsonConvert.DeserializeObject<List<MutualFund>>(tickerJson);


            var dataTable = new DataTable("MutualFunds");

            // Add the columns in the temp table
            dataTable.Columns.Add("CUSIP");
            dataTable.Columns.Add("Ticker", typeof(string));
            dataTable.Columns.Add("Description");
            dataTable.Columns.Add("LastPrice");
            dataTable.Columns.Add("Yield");
            dataTable.Columns.Add("AssetClass", typeof(string));
            dataTable.Columns.Add("Category", typeof(string));
            dataTable.Columns.Add("Subcategory", typeof(string));

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                // Truncate the live table
                using (var sqlCommand = new SqlCommand(_truncateMutualFundLiveTableCommandText, sqlConnection))
                {
                    sqlCommand.ExecuteNonQuery();
                }


                var sqlBulkCopy = new SqlBulkCopy(sqlConnection)
                {
                    DestinationTableName = "MutualFunds"

                };

                foreach (var fund in mutualFundList)
                {
                    dataTable.Rows.Add("", fund.Ticker, "", fund.LastPrice, fund.Yield, fund.AssetClass, fund.Category, fund.Subcategory);
                }

                InsertDataTable(sqlBulkCopy, sqlConnection, dataTable);

                sqlConnection.Close();
            }
        }

        public void LoadStockJsonDataIntoSqlServer(string connectionString)
        {
            //Can I find a better way that deserializing to the StockTicker class with one property?
            var fileInfo = new FileInfo(@"SeedJson\SeedTickers.json");
            var tickerJson = File.ReadAllText(fileInfo.FullName);
            stockList = JsonConvert.DeserializeObject<List<SecurityClasses.StockFromJSON>>(tickerJson);
                            

            var dataTable = new DataTable("Stocks");

            // Add the columns in the temp table
            dataTable.Columns.Add("CUSIP");
            dataTable.Columns.Add("Ticker", typeof(string));
            dataTable.Columns.Add("Description", typeof(string));
            dataTable.Columns.Add("LastPrice");
            dataTable.Columns.Add("Yield");

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();

                // Truncate the live table
                using (var sqlCommand = new SqlCommand(_truncateStockLiveTableCommandText, sqlConnection))
                {
                    sqlCommand.ExecuteNonQuery();
                }

                var sqlBulkCopy = new SqlBulkCopy(sqlConnection)
                {
                    DestinationTableName = "[Stocks]"     
                };

                foreach (var security in stockList)
                {
                    dataTable.Rows.Add(null, security.Ticker, security.Description, null, null);
                }

               InsertDataTable(sqlBulkCopy, sqlConnection, dataTable);

                sqlConnection.Close();
            }
            
        }

        protected void InsertDataTable(SqlBulkCopy sqlBulkCopy, SqlConnection sqlConnection, DataTable dataTable)
        {
            sqlBulkCopy.WriteToServer(dataTable);
            dataTable.Rows.Clear();
        }

        public void Dispose()
        {
            stockList = null;
        }
    }
}
