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
        private List<SecurityClasses.StockTicker> tickerList;
        protected const string _truncateLiveTableCommandText = @"TRUNCATE TABLE [Stocks]"; //My table name 
        protected const int _batchSize = 2000; //max number times this look to add. Adjust for need vs. speed.

        public SecurityTableSeederDataService()
        {
            
        }

        public void LoadJsonDataIntoSqlServer(string connection)
        {

            //var jsonFi = new FileInfo(@"Assets\Games.json");
            //return jsonFi.Exists
            //    ?
            //    JsonConvert.DeserializeObject<ObservableCollection<Game>>(File.ReadAllText(jsonFi.FullName))
            //    : null;

            var fileInfo = new FileInfo(@"SeedJson\SeedTickers.json");
            var tickerJson = File.ReadAllText(fileInfo.FullName);
            tickerList = JsonConvert.DeserializeObject<List<SecurityClasses.StockTicker>>(tickerJson);
                            

            var dataTable = new DataTable("Stocks");

            // Add the columns in the temp table
            dataTable.Columns.Add("CUSIP");
            dataTable.Columns.Add("Ticker");
            dataTable.Columns.Add("Description");
            dataTable.Columns.Add("LastPrice");
            dataTable.Columns.Add("Yield");

            using (var sqlConnection = new SqlConnection(connection))
            {
                sqlConnection.Open();

                // Truncate the live table
                using (var sqlCommand = new SqlCommand(_truncateLiveTableCommandText, sqlConnection))
                {
                    sqlCommand.ExecuteNonQuery();
                }

   
               var sqlBulkCopy = new SqlBulkCopy(sqlConnection)
               {
                   DestinationTableName = "[Stocks]"
               };

                //Setup the column mappings, anything ommitted is skipped
                sqlBulkCopy.ColumnMappings.Add("CUSIP", "CUSIP");
                sqlBulkCopy.ColumnMappings.Add("Ticker", "Ticker");
                sqlBulkCopy.ColumnMappings.Add("Description", "Description");
                sqlBulkCopy.ColumnMappings.Add("LastPrice", "LastPrice");
                sqlBulkCopy.ColumnMappings.Add("Yield", "Yield");

                foreach (var ticker in tickerList)
                {
                    dataTable.Rows.Add("", ticker, "", 0.00, 0.00);
                }

                //failing on nchar/string conversion
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
            tickerList = null;
        }
    }
}
