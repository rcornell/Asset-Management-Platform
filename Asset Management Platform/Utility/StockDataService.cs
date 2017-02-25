using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data.SqlClient;
using GalaSoft.MvvmLight.Messaging;
using Asset_Management_Platform.Messages;
using System.Linq;
using System.Threading.Tasks;
using Asset_Management_Platform.SecurityClasses;


namespace Asset_Management_Platform.Utility
{
    /// <summary>
    /// This class is designed to communicate with an SQL database 
    /// containing basic data for all the positions covered by
    /// this application.
    /// </summary>
    public class StockDataService : IStockDataService
    {        
        private readonly string _storageString;
        private readonly bool _localMode;
        private List<Security> _securityDatabaseList;

        public StockDataService()
        {
            Messenger.Default.Register<StockDataRequestMessage>(this, HandleStockDataRequest);
            Messenger.Default.Register<StartupCompleteMessage>(this, HandleStartupComplete);

            _securityDatabaseList = new List<Security>();
            _storageString = ConfigurationManager.AppSettings["StorageConnectionString"];
            _localMode = _storageString == null ? true : false;            
        }

        private async void HandleStartupComplete(StartupCompleteMessage message)
        {
            if (!message.IsComplete)
                return;

            Messenger.Default.Send<LocalModeMessage>(new LocalModeMessage(_localMode));

            if (!_localMode)
            {
                await CheckDatabases();
            }

            await LoadSecurityDatabase();
        }

        private async void HandleStockDataRequest(StockDataRequestMessage message)
        {
            if (message.Tickers != null || message.Positions != null) //Request for multiple securities
                await GetSecurityInfo(message);
            if (!string.IsNullOrEmpty(message.Ticker)) //ticker property is not null, request for one security
            {
                var isScreener = message.IsScreenerRequest;
                var isPreview = message.IsTradePreviewRequest;
                await GetSecurityInfo(message.Ticker, isScreener,  isPreview);                
            }
        }

        /// <summary>
        /// Reads the SQL database and sends a message with the List<Security>
        /// of known securities
        /// </summary>
        private async Task LoadSecurityDatabase()
        {

            if (_localMode) { 
                Messenger.Default.Send(new SecurityDatabaseMessage(_securityDatabaseList));
                return;
            }

            using (var connection = new SqlConnection(_storageString))
            {
                connection.Open();
                var stocks = await LoadStocksFromDB(connection);
                var funds = await LoadMutualFundsFromDB(connection);

                _securityDatabaseList.AddRange(stocks);
                _securityDatabaseList.AddRange(funds);
            }

            _securityDatabaseList = GetMutualFundExtraData(_securityDatabaseList);
            
            //Listener is PortfolioManagementService
            Messenger.Default.Send<SecurityDatabaseMessage>(new SecurityDatabaseMessage(_securityDatabaseList));
        }

        /// <summary>
        /// Insert position(s) into the database and returns true if successful
        /// </summary>
        /// <param name="securitiesToInsert"></param>
        /// <returns></returns>
        public async void TryDatabaseInsert(Security securityToInsert)
        {
            var isInDatabase = _securityDatabaseList.Any(s => s.Ticker == securityToInsert.Ticker);

            if (isInDatabase || securityToInsert.Ticker == @"N/A")
                return;

            if (securityToInsert.SecurityType == "Stock")
            {
                var insertString = @"INSERT INTO Stocks (Ticker, Description, LastPrice, Yield) VALUES ";
                using (var connection = new SqlConnection(_storageString))
                {
                    securityToInsert.Description = securityToInsert.Description.Replace(@"'", @"");
                    var securityInfo = string.Format(@"('{0}', '{1}', {2}, {3});", securityToInsert.Ticker, securityToInsert.Description,
                                                                                  securityToInsert.LastPrice, securityToInsert.Yield);
                    insertString += securityInfo;

                    using (var command = new SqlCommand(insertString, connection))
                    {
                        connection.Open();
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            else if (securityToInsert is MutualFund)
            {
                var fund = (MutualFund)securityToInsert;
                var insertString = @"INSERT INTO MutualFunds (Ticker, Description, LastPrice, Yield, AssetClass, Category, Subcategory) VALUES ";
                using (var connection = new SqlConnection(_storageString))
                {
                    var securityInfo = string.Format(@"('{0}', '{1}', {2}, {3}, '{4}', '{5}', '{6}')", securityToInsert.Ticker, securityToInsert.Description,
                        securityToInsert.LastPrice, securityToInsert.Yield, fund.AssetClass, fund.Category, fund.Subcategory);

                    insertString += securityInfo;

                    using (var command = new SqlCommand(insertString, connection))
                    {                       
                        connection.Open();
                        await command.ExecuteNonQueryAsync();
                    }                    
                }
            }
            _securityDatabaseList.Add(securityToInsert);
        }

        /// <summary>
        /// Uploads table to database upon closing.
        /// </summary>
        /// <returns></returns>
        public async void UploadSecuritiesToDatabase()
        {
            if (_securityDatabaseList == null || _securityDatabaseList.Count == 0)
                return;

            using (var connection = new SqlConnection(_storageString))
            {
                connection.Open();

                await UploadStocksToDb(connection);
                await UploadFundsToDb(connection);
            }            
        }

        private async Task UploadFundsToDb(SqlConnection connection)
        {
            //Truncate MF table
            var deleteMFCommand = @"TRUNCATE TABLE dbo.MutualFunds;";
            using (var command = new SqlCommand(deleteMFCommand, connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            //Create the VALUES portion of the SQL Insert
            var fundInsertBase = @"INSERT INTO MutualFunds (Ticker, Description, LastPrice, Yield, AssetClass, Category, Subcategory) VALUES ";
            string fundValues = "";
            foreach (var sec in _securityDatabaseList)
            {
                if (sec is MutualFund)
                {
                    var fund = (MutualFund)sec;
                    var newValue = string.Format(@"('{0}', '{1}', {2}, '{3}', '{4}', '{5}', '{6}'), ", fund.Ticker, fund.Description.Replace("'", ""),
                        fund.LastPrice, fund.Yield, fund.AssetClass, fund.Category, fund.Subcategory);
                    fundValues += newValue;
                }
            }

            //Insert the funds to the table
            if (!string.IsNullOrEmpty(fundValues))
            {
                fundValues = fundValues.Substring(0, fundValues.Length - 2) + @";";
                fundInsertBase += fundValues;
                using (var command = new SqlCommand(fundInsertBase, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task UploadStocksToDb(SqlConnection connection)
        {
            //Clear the table of stocks
            var deleteStockCommand = @"TRUNCATE TABLE dbo.Stocks;";
            using (var command = new SqlCommand(deleteStockCommand, connection))
            {
                await command.ExecuteNonQueryAsync();
            }

            //Create the VALUES for the SQL Insert
            var stockInsertBase = @"INSERT INTO Stocks (Cusip, Ticker, Description, LastPrice, Yield) VALUES ";
            string stockValues = "";
            foreach (var sec in _securityDatabaseList)
            {
                if (sec is Stock)
                {
                    var newValue = string.Format("('{0}', '{1}', '{2}', {3}, {4}), ", sec.Cusip, sec.Ticker, sec.Description.Replace("'", ""), sec.LastPrice, sec.Yield);
                    stockValues += newValue;
                }
            }

            //Insert stocks into DB
            if (!string.IsNullOrEmpty(stockValues))
            {
                stockValues = stockValues.Substring(0, stockValues.Length - 2) + @";";
                stockInsertBase += stockValues;

                using (var command = new SqlCommand(stockInsertBase, connection))
                {
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Get pricing and security info for a single ticker.
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public async Task GetSecurityInfo(string ticker, bool isScreener, bool isPreview)
        {
            if (!string.IsNullOrEmpty(ticker))
            {
                using (var yahooAPI = new YahooAPIService())
                {
                    var result = await yahooAPI.GetSingleSecurity(ticker, _securityDatabaseList);

                    if(!_localMode)
                        TryDatabaseInsert(result);

                    var responseMessage = new StockDataResponseMessage(result, isPreview, isScreener);
                    Messenger.Default.Send<StockDataResponseMessage>(responseMessage);
                }
            }
        }

        /// <summary>
        /// Method takes a request for multiple tickers or multiple positions
        /// and sends a listed of updated data
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task GetSecurityInfo(StockDataRequestMessage message)
        {
            var tickersToQuery = new List<string>();

            var positionsQuery = false;
            var tickersQuery = false;

            if (message.Tickers != null && message.Tickers.Count > 0)
            { 
                tickersToQuery = message.Tickers;
                tickersQuery = true;
            }
            else if (message.Positions != null && message.Positions.Count > 0)
            {
                tickersToQuery = message.Positions.Select(s => s.Ticker).Distinct().ToList();
                positionsQuery = true;
            }

            using (var yahooAPI = new YahooAPIService())
            {
                var resultList = await yahooAPI.GetMultipleSecurities(tickersToQuery);
                    
                //Return response. Message's boolean is True if this is a startup call of this method.
                if (message.IsStartupRequest && positionsQuery)
                    Messenger.Default.Send<PositionPricingMessage>(new PositionPricingMessage(resultList, true));
                else if(message.IsStartupRequest && tickersQuery)
                    Messenger.Default.Send<StockDataResponseMessage>(new StockDataResponseMessage(resultList, true));
                else
                    Messenger.Default.Send<StockDataResponseMessage>(new StockDataResponseMessage(resultList, false));
            }            
        }

        public List<Security> GetMutualFundExtraData(List<Security> rawSecurities)
        {
            if (_localMode)
                return rawSecurities;

            var updatedSecurities = new List<Security>();
            var funds = rawSecurities.Where(s => s is MutualFund);

            foreach (var fund in funds.Cast<MutualFund>())
            {
                if (_securityDatabaseList.Any(f => f.Ticker == fund.Ticker))
                {
                    var fundFromDB = (MutualFund)_securityDatabaseList.Find(m => m.Ticker == fund.Ticker);

                    fund.AssetClass = fundFromDB.AssetClass;
                    fund.Category = fundFromDB.Category;
                    fund.Subcategory = fundFromDB.Subcategory;
                }
            }

            updatedSecurities.AddRange(rawSecurities.Where(p => p is Stock));
            updatedSecurities.AddRange(funds);

            return updatedSecurities;
        }

        /// <summary>
        /// Called by UpdatePortfolioPrices and LimitOrderChecks
        /// </summary>
        /// <param name="securities"></param>
        /// <returns></returns>
        public async Task GetUpdatedPricing(List<Security> securities)
        {
            var resultList = new List<Security>();
            if (securities != null && securities.Count > 0)
            {
                using (var yahooAPI = new YahooAPIService())
                {
                    await yahooAPI.GetUpdatedPricing(securities);
                }
            }
        }

        public async Task GetUpdatedPricing(List<Position> positions)
        {
            var secList = new List<Security>();

            foreach (var pos in positions)
            {
                secList.Add(pos.Security);
            }


            if (positions != null && positions.Count > 0)
            {
                using (var yahooAPI = new YahooAPIService())
                {
                    await yahooAPI.GetUpdatedPricing(positions);
                }
            }
        }

        private async Task<List<Security>> LoadMutualFundsFromDB(SqlConnection connection)
        {
            var securityList = new List<Security>();
            var commandText = @"SELECT * FROM MUTUALFUNDS";

            using (var command = new SqlCommand(commandText, connection))
            {
                string cusip = "";
                string ticker = "";
                string description = "";
                decimal lastPrice = 0;
                double yield = 0;
                string assetClass = "";
                string category = "";
                string subCategory = "";

                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        cusip = string.IsNullOrEmpty(reader.GetString(0)) ? "" : reader.GetString(0);
                    if (!reader.IsDBNull(1))
                        ticker = string.IsNullOrEmpty(reader.GetString(1)) ? "" : reader.GetString(1);
                    if (!reader.IsDBNull(2))
                        description = string.IsNullOrEmpty(reader.GetString(2)) ? "" : reader.GetString(2);
                    if (!reader.IsDBNull(3))
                        lastPrice = decimal.Parse(reader.GetString(3));
                    if (!reader.IsDBNull(4))
                        yield = double.Parse(reader.GetString(4));
                    if (!reader.IsDBNull(5))
                        assetClass = string.IsNullOrEmpty(reader.GetString(5)) ? "" : reader.GetString(5);
                    if (!reader.IsDBNull(6))
                        category = string.IsNullOrEmpty(reader.GetString(6)) ? "" : reader.GetString(6);
                    if (!reader.IsDBNull(7))
                        subCategory = string.IsNullOrEmpty(reader.GetString(7)) ? "" : reader.GetString(7);
                    securityList.Add(new MutualFund(cusip, ticker, description, lastPrice, yield, assetClass, category, subCategory));
                }
                reader.Close();
            }
            return securityList;
        }

        private async Task<List<Security>> LoadStocksFromDB(SqlConnection connection)
        {
            var securityList = new List<Security>();
            var commandText = @"SELECT * FROM STOCKS";

            using (var command = new SqlCommand(commandText, connection))
            {
                string cusip = "";
                string ticker = "";
                string description = "";
                decimal lastPrice = 0;
                double yield = 0;

                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        cusip = string.IsNullOrEmpty(reader.GetString(0)) ? "" : reader.GetString(0);
                    if (!reader.IsDBNull(1))
                        ticker = string.IsNullOrEmpty(reader.GetString(1)) ? "" : reader.GetString(1);
                    if (!reader.IsDBNull(2))
                        description = string.IsNullOrEmpty(reader.GetString(2)) ? "" : reader.GetString(2);
                    if (!reader.IsDBNull(3))
                        lastPrice = decimal.Parse(reader.GetString(3));
                    if (!reader.IsDBNull(4))
                        yield = double.Parse(reader.GetString(4));
                    securityList.Add(new Stock(cusip, ticker, description, lastPrice, yield));
                }
                reader.Close();
            }
            return securityList;
        }

        /// <summary>
        /// Check to see if SQL Database for stocks is empty.
        /// </summary>
        private async Task<bool> IsStockDatabaseEmpty()
        {
            var result = 0;
            var cmdText = @"SELECT COUNT(*) from Stocks";

            using (var connection = new SqlConnection(_storageString))
            {
                connection.Open();
                using (var command = new SqlCommand(cmdText, connection))
                {
                        var response = await command.ExecuteScalarAsync();
                        int.TryParse(response.ToString(), out result);          
                }
            }

            if (result > 0)
                return false; //Database IS NOT empty

            return true; //Database IS empty
        }

        /// <summary>
        /// Check to see if SQL Database for funds is empty.
        /// </summary>
        private async Task<bool> IsMutualFundDatabaseEmpty()
        {
            var result = 0;
            var cmdText = @"SELECT COUNT(*) from MutualFunds";

            using (var connection = new SqlConnection(_storageString))
            {
                connection.Open();
                using (var command = new SqlCommand(cmdText, connection))
                {
                    var response = await command.ExecuteScalarAsync();
                    int.TryParse(response.ToString(), out result);
                }
            }

            if (result > 0)
                return false; //Database IS NOT empty            

            return true; //Database IS empty
        }

        /// <summary>
        /// Checks the SQL databases for stocks and funds. If empty, seeds it.
        /// </summary>
        private async Task CheckDatabases()
        {
            if (await IsStockDatabaseEmpty())
            {
                await SeedStockDatabase();
                Messenger.Default.Send(new PortfolioSqlMessage("Empty database restored.", false));
            }
            if (await IsMutualFundDatabaseEmpty())
            {
                await SeedMutualFundDatabase();
                Messenger.Default.Send(new PortfolioSqlMessage("Empty database restored.", false));
            }
        }

        /// <summary>
        /// Seeds the SQL table if it has no contents using 
        /// local SeedTicker.json file.
        /// </summary>
        private async Task SeedStockDatabase()
        {
            using (var seeder = new SecurityTableSeederDataService())
            {
                await seeder.LoadStockJsonDataIntoSqlServer(_storageString);
            }
        }

        private async Task SeedMutualFundDatabase()
        {
            using (var seeder = new SecurityTableSeederDataService())
            {
                await seeder.LoadMutualFundJsonDataIntoSqlServer(_storageString);
            }
        }
    }
}
