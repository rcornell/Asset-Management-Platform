using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.VisualBasic.FileIO;

namespace Asset_Management_Platform.Utility
{
    class SecurityTableSeederDataService : IDisposable
    {

        protected const string _truncateLiveTableCommandText = @"TRUNCATE TABLE Stocks"; //My table name 
        protected const int _batchSize = 2000; //max number times this look to add. Adjust for need vs. speed.

        public SecurityTableSeederDataService()
        {
            
        }

        public void LoadCsvDataIntoSqlServer(string connection)
        {   //NOT READY TO BE USED
            // This should be the full path within the project, which will include the CSV file
            var fileName = @"C:\Users\Rob\Documents\StockListCSV.csv";

            var createdCount = 0;

            using (var textFieldParser = new TextFieldParser(fileName))
            {
                textFieldParser.TextFieldType = FieldType.Delimited;
                textFieldParser.Delimiters = new[] { "," };
                textFieldParser.HasFieldsEnclosedInQuotes = true;

                var dataTable = new DataTable("Stocks");

                // Add the columns in the temp table
                dataTable.Columns.Add("Ticker");

                using (var sqlConnection = new SqlConnection(connection))
                {
                    sqlConnection.Open();

                    // Truncate the live table
                    using (var sqlCommand = new SqlCommand(_truncateLiveTableCommandText, sqlConnection))
                    {
                        sqlCommand.ExecuteNonQuery();
                    }

                    // Create the bulk copy object
                    var sqlBulkCopy = new SqlBulkCopy(sqlConnection)
                    {
                        DestinationTableName = "Stocks"
                    };

                    // Setup the column mappings, anything ommitted is skipped
                    sqlBulkCopy.ColumnMappings.Add("CUSIP", "CUSIP");
                    sqlBulkCopy.ColumnMappings.Add("Ticker", "Ticker");
                    sqlBulkCopy.ColumnMappings.Add("Description", "Description");
                    sqlBulkCopy.ColumnMappings.Add("LastPrice", "LastPrice");
                    sqlBulkCopy.ColumnMappings.Add("Yield", "Yield");

                    // Loop through the CSV and load each set of 500 records into a DataTable
                    // Then send it to the LiveTable
                    while (!textFieldParser.EndOfData)
                    {
                        dataTable.Rows.Add(textFieldParser.ReadFields());

                        createdCount++;

                        if (createdCount % _batchSize == 0)
                        {
                            InsertDataTable(sqlBulkCopy, sqlConnection, dataTable);

                            break;
                        }
                    }

                    // Don't forget to send the last batch under 100,000
                    InsertDataTable(sqlBulkCopy, sqlConnection, dataTable);

                    sqlConnection.Close();
                }
            }
        }

        protected void InsertDataTable(SqlBulkCopy sqlBulkCopy, SqlConnection sqlConnection, DataTable dataTable)
        {
            sqlBulkCopy.WriteToServer(dataTable);

            dataTable.Rows.Clear();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
