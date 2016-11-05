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

namespace Asset_Management_Platform.Utility
{
    class SecurityTableSeederDataService : IDisposable
    {
        private List<string> tickerList;
        protected const string _truncateLiveTableCommandText = @"TRUNCATE TABLE [Stocks]"; //My table name 
        protected const int _batchSize = 2000; //max number times this look to add. Adjust for need vs. speed.

        public SecurityTableSeederDataService()
        {
            
        }

        public void LoadJsonDataIntoSqlServer(string connection)
        {  
            var tickerJson = ConfigurationManager.AppSettings["SeedTicker"];
            tickerList = JsonConvert.DeserializeObject<List<string>>(tickerJson);

            var dataTable = new DataTable("Stocks");

            // Add the columns in the temp table
            dataTable.Columns.Add("CUSIP", typeof(string));
            dataTable.Columns.Add("Ticker", typeof(string));
            dataTable.Columns.Add("Description", typeof(string));
            dataTable.Columns.Add("LastPrice", typeof(float));
            dataTable.Columns.Add("Yield", typeof(double));

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
