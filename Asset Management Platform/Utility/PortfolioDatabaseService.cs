using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight;
using System.Text;
using System.Threading.Tasks;
using Asset_Management_Platform.Messages;
using Asset_Management_Platform.Utility;
using System.Configuration;
using Asset_Management_Platform.Exceptions;
using Asset_Management_Platform.SecurityClasses;

namespace Asset_Management_Platform
{
    /// <summary>
    /// Manages the logic of the user's positions and taxlots
    /// </summary>
    public class PortfolioDatabaseService : IPortfolioDatabaseService
    {
        private readonly string _storageString;
        private readonly List<Position> _portfolioOriginalState;
        private readonly List<Position> _myPositions; //This is THE main position list
        private readonly List<Taxlot> _myTaxlots; //This is THE main taxlot list
        private List<LimitOrder> _myLimitOrders;
        private bool _localMode;
        private IStockDataService _stockDatabaseService;    
       
        public PortfolioDatabaseService(IStockDataService stockDatabaseService)
        {
            //Register for LocalMode notification
            Messenger.Default.Register<LocalModeMessage>(this, SetLocalMode);

            Messenger.Default.Register<TradeBuyMessage>(this, HandleBuy);
            Messenger.Default.Register<TradeSellMessage>(this, HandleSell);
            Messenger.Default.Register<LimitOrderMessage>(this, HandleLimitOrderList);

            _stockDatabaseService = stockDatabaseService;

            _portfolioOriginalState = new List<Position>();
            _myPositions = new List<Position>();
            _myTaxlots = new List<Taxlot>();
            _myLimitOrders = new List<LimitOrder>();

            LoadLimitOrdersFromDatabase();
            BuildDatabaseTaxlots();

        }

        private void HandleLimitOrderList(LimitOrderMessage message)
        {
            //Only update the List<LimitOrder> field in this class
            //If a LimitOrderMessage is sent during runtime
            if (!message.IsStartup)
            {
                _myLimitOrders = message.LimitOrders;
            }
        }

        public async Task BuildDatabaseTaxlots()
        {
            var downloadedTaxlots = new List<Taxlot>();

            if (_localMode)
                Messenger.Default.Send<TaxlotMessage>(new TaxlotMessage(downloadedTaxlots));

            using (var connection = new SqlConnection(_storageString))
            {
                var commandText = @"SELECT * FROM MyPortfolio;";

                using (var command = new SqlCommand(commandText, connection))
                {
                    connection.Open();
                    var reader = await Task.Run(() => command.ExecuteReader());
                    while (reader.Read())
                    {
                        Security secType;
                        var datePurchased = new DateTime();

                        var ticker = reader.GetString(1);
                        var quantity = int.Parse(reader.GetString(2));
                        var purchasePrice = decimal.Parse(reader.GetString(3));                        
                        datePurchased = reader.IsDBNull(4) ? new DateTime(2000, 12, 31) : DateTime.Parse(reader.GetString(4));
                        var securityTypeResult = reader.GetString(5);
                        if (securityTypeResult == "Stock")
                            secType = new Stock();
                        else
                            secType = new MutualFund();
                        var taxLot = new Taxlot(ticker, quantity, purchasePrice, datePurchased, secType);
                        _myTaxlots.Add(taxLot);
                    }
                }
            }
            Messenger.Default.Send<TaxlotMessage>(new TaxlotMessage(downloadedTaxlots));
        }

        public List<Taxlot> BuildLocalTaxlots(List<Taxlot> taxlots)
        {
            if (taxlots == null)
                return new List<Taxlot>(); //Should not hit this

            foreach (var lot in taxlots)
            {
                _myTaxlots.Add(new Taxlot(lot.Ticker, lot.Shares, lot.PurchasePrice, lot.DatePurchased, lot.SecurityType));
            }
            return _myTaxlots;
        }

        /// <summary>
        /// This method is called if NOT in local mode.
        /// </summary>
        /// <param name="portfolioSecurities"></param>
        /// <returns></returns>
        public List<Position> GetPositionsFromTaxlots(List<Security> portfolioSecurities)
        {
            foreach (var lot in _myTaxlots)
            {
                if (!_myPositions.Any(s => s.Ticker == lot.Ticker))
                {   
                    //No position in _myPositions with this ticker yet.
                    var security = portfolioSecurities.Find(t => t.Ticker == lot.Ticker);
                    _myPositions.Add(new Position(lot, security));
                }
                else if (_myPositions.Any(s => s.Ticker == lot.Ticker && s.SharesOwned != lot.Shares))
                {
                    //Position with this ticker exists but the taxlot share quantity is different.
                    _myPositions.Find(s => s.Ticker == lot.Ticker).AddTaxlot(lot);
                }
                else if (_myPositions.Any(s => s.Ticker == lot.Ticker && s.SharesOwned == lot.Shares))
                {
                    //Ticker and share quantities match. Check date to see if duplicate taxlot.
                    var pos = _myPositions.Find(s => s.Ticker == lot.Ticker);
                    if (!pos.Taxlots.Any(d => d.DatePurchased == lot.DatePurchased))
                        pos.AddTaxlot(lot);
                }
            }

            foreach (var pos in _myPositions)
            {
                //Create the list of positions at startup for use in shutdown methods.
                _portfolioOriginalState.Add(new Position(pos.Taxlots));
            }

            return _myPositions;
        }

        /// <summary>
        /// This method is called if in local mode.
        /// </summary>
        /// <returns></returns>
        public void GetPositionsFromTaxlots()
        {
            foreach (var lot in _myTaxlots)
            {
                if (!_myPositions.Any(s => s.Ticker == lot.Ticker))
                {
                    //No position in _myPositions with this ticker yet.
                    _myPositions.Add(new Position(lot, lot.SecurityType));
                }
                else if (_myPositions.Any(s => s.Ticker == lot.Ticker && s.SharesOwned != lot.Shares))
                {
                    //Position with this ticker exists but the taxlot share quantity is different.
                    _myPositions.Find(s => s.Ticker == lot.Ticker).AddTaxlot(lot);
                }
                else if (_myPositions.Any(s => s.Ticker == lot.Ticker && s.SharesOwned == lot.Shares))
                {
                    //Ticker and share quantities match. Check date to see if duplicate taxlot.
                    var pos = _myPositions.Find(s => s.Ticker == lot.Ticker);
                    if (!pos.Taxlots.Any(d => d.DatePurchased == lot.DatePurchased))
                        pos.AddTaxlot(lot);
                }
            }

            foreach (var pos in _myPositions)
            {
                //Create the list of positions at startup for use in shutdown methods.
                _portfolioOriginalState.Add(new Position(pos.Taxlots));
            }

            //Return new List<Position> to all Startup listeners
            Messenger.Default.Send<PositionMessage>(new PositionMessage(_myPositions, true));
        }

        /// <summary>
        /// Compares _myPositions to the _databaseOriginalState from launch
        /// and creates lists of securities to update, insert, or delete.
        /// </summary>
        public void SavePortfolioToDatabase()
        {
            BackupDatabase();

            var positionsToInsert = new List<Position>();
            var positionsToUpdate = new List<Position>();
            var positionsToDelete = new List<Position>();

            foreach (var pos in _portfolioOriginalState)
            {
                if (!_myPositions.Any(s => s.Ticker == pos.Ticker))
                    positionsToDelete.Add(pos);
            }

            foreach (var p in _myPositions)
            {
                //Current position's ticker is in the original list but the current quantity is different
                if (_portfolioOriginalState.Any(pos => pos.Ticker == p.Ticker && pos.SharesOwned != p.SharesOwned))
                {
                    positionsToUpdate.Add(p);
                    continue;
                }

                //Current position's ticker not in original list
                if (!_portfolioOriginalState.Any(pos => pos.Ticker == p.Ticker))
                {
                    positionsToInsert.Add(p);
                    continue;
                }

                //Current position share quantity matches original list. 
                //Check if taxlots are different or the same as original.
                if (_portfolioOriginalState.Any(pos => pos.Ticker == p.Ticker && pos.SharesOwned == p.SharesOwned))
                {
                    var originalPosition = _portfolioOriginalState.Find(s => s.Ticker == p.Ticker);

                    if (originalPosition != null && !TaxLotsAreEqual(originalPosition, p))
                        positionsToUpdate.Add(p);
                }
            }

            //If no inserts, updates, or deletes, exit method.
            if (positionsToInsert.Count == 0 && positionsToUpdate.Count == 0 && positionsToDelete.Count == 0)
                return;

            try
            {
                if (positionsToUpdate.Any())
                {
                    UpdatePositions(positionsToUpdate);
                }

                if (positionsToInsert.Any())
                {
                    InsertPositions(positionsToInsert);
                }

                if (positionsToDelete.Any())
                {
                    DeletePositions(positionsToDelete);
                }
            }
            catch (SqlException ex)
            {
                var msg = new PortfolioSqlMessage(ex.Message, false);
                Messenger.Default.Send(msg);
            }
            catch (InvalidOperationException ex)
            {
                var msg = new PortfolioSqlMessage(ex.Message, false);
                Messenger.Default.Send(msg);
            }
        }

        private void InsertPositions(List<Position> positions)
        {
            
            string insertString = @"INSERT INTO dbo.MyPortfolio (Ticker, Shares, CostBasis, DatePurchased, SecurityType) VALUES ";

            foreach (var pos in positions)
            {
                insertString = pos.Taxlots.Aggregate(insertString, (current, lot) => current + string.Format("('{0}', '{1}', '{2}', '{3}', '{4}'), ", lot.Ticker, lot.Shares, lot.PurchasePrice, lot.DatePurchased, lot.SecurityType));
            }
            insertString = insertString.Substring(0, insertString.Length - 2) + @";";

            using (var connection = new SqlConnection(_storageString))
            {
                connection.Open();
                using (var command = new SqlCommand(insertString, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void UpdatePositions(List<Position> positions)
        {
            //Delete the taxlots/positions so they can be re-added
            DeletePositions(positions);

            string insertString = @"INSERT INTO dbo.MyPortfolio (Ticker, Shares, CostBasis, DatePurchased, SecurityType) VALUES ";

            foreach (var pos in positions)
            {
                //Re-adds all current taxlots
                foreach (var lot in pos.Taxlots)
                {
                    insertString += string.Format(@"('{0}' ,'{1}' ,'{2}' , '{3}', '{4}'), ", lot.Ticker, lot.Shares, lot.PurchasePrice, lot.DatePurchased, lot.SecurityType);
                }
            }
            insertString = insertString.Substring(0, insertString.Length - 2) + @";";

            using (var connection = new SqlConnection(_storageString))
            {
                connection.Open();
                using (var command = new SqlCommand(insertString, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private void DeletePositions(List<Position> positions)
        {
            var deleteString = positions.Aggregate(@"Delete From dbo.MyPortfolio Where Ticker in (", (current, pos) => current + string.Format(@"'{0}', ", pos.Ticker)); //value1, value2, ...);

            deleteString = deleteString.Substring(0, deleteString.Length - 2);
            deleteString += @");";

            using (var connection = new SqlConnection(_storageString))
            {
                connection.Open();
                using (var command = new SqlCommand(deleteString, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Compares properties of two positions to see if they are the same
        /// </summary>
        /// <param name="originalPosition"></param>
        /// <param name="portfolioPosition"></param>
        /// <returns></returns>
        private bool TaxLotsAreEqual(Position originalPosition, Position portfolioPosition)
        {
            if (originalPosition.Taxlots.Count != portfolioPosition.Taxlots.Count)
                return false;
            if (originalPosition.PurchasePrice != portfolioPosition.PurchasePrice)
                return false;

            for (var i = 0; i < portfolioPosition.Taxlots.Count; i++)
            {
                var originalTaxlotDateTime = originalPosition.Taxlots[i].DatePurchased;
                var portfolioTaxlotDateTime = portfolioPosition.Taxlots[i].DatePurchased;

                if (originalTaxlotDateTime.CompareTo(portfolioTaxlotDateTime) != 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Creates a copy of MyPortfolio table in Database in case of update error.
        /// </summary>
        public void BackupDatabase()
        {
            //Perhaps a way to create multiple backups?

            TruncateTable("MyPortfolioBackup");

            var backup = @"INSERT INTO MyPortfolioBackup SELECT * FROM MyPortfolio";
            using (var connection = new SqlConnection(_storageString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = backup;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UploadLimitOrdersToDatabase()
        {
            var insertString = @"INSERT INTO dbo.MyLimitOrders (TradeType, Ticker, Shares, Limit, SecurityType, OrderDuration) VALUES ";

            var final = _myLimitOrders.Last();
            foreach (var order in _myLimitOrders)
            {
                insertString += string.Format(@"('{0}', '{1}', {2}, {3}, '{4}', '{5}')", order.TradeType, order.Ticker, order.Shares, order.Limit, order.SecurityType, order.OrderDuration);
                if (order != final)
                    insertString += @", ";
            }
            try { 
                using (var connection = new SqlConnection(_storageString))
                {
                    connection.Open();
                    var selectAllString = @"SELECT * FROM MyLimitOrders";

                    using (var selectCommand = new SqlCommand(selectAllString, connection))
                    {
                        var reader = selectCommand.ExecuteReader();
                        if (reader.HasRows) {
                            TruncateTable("LimitOrders");
                        }
                    }    
                }

                using (var connection = new SqlConnection(_storageString))
                {
                    connection.Open();

                    using (var insertCommand = new SqlCommand(insertString, connection))
                    {
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                var message = "Limit orders failed to upload properly";
                throw new LimitOrderException(message, ex);
            }
        }

        /// <summary>
        /// Truncates the specified table
        /// </summary>
        /// <param name="tableToTruncate"></param>
        private void TruncateTable(string tableToTruncate)
        {
            string truncateString = "";
            switch (tableToTruncate)
            {
                case "LimitOrders":
                    truncateString = @"TRUNCATE TABLE MyLimitOrders;";
                    break;
                case "MyPortfolio":
                    truncateString = @"TRUNCATE TABLE MyPortfolio;";
                    break;
                case "MyPortfolioBackup":
                    truncateString = @"TRUNCATE TABLE MyPortfolioBackup;";
                    break;
            }
            try
            {
                using (var truncateConnection = new SqlConnection(_storageString))
                {
                    truncateConnection.Open();
                    using (var truncateCommand = new SqlCommand(truncateString, truncateConnection))
                    {
                        truncateCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                var message = @"Failed to truncate SQL table: " + truncateString;
                throw new TruncateException(message, ex);
            }
            
        }

        /// <summary>
        /// Downloads and parses the list of limit orders from SQL table
        /// </summary>
        /// <returns></returns>
        public async Task LoadLimitOrdersFromDatabase()
        {
            var downloadString = @"SELECT * FROM dbo.MyLimitOrders;";
            var limitDBResults = new List<LimitOrderDbResult>();

            using (var connection = new SqlConnection(_storageString))
            {
                using (var command = new SqlCommand(downloadString, connection))
                {
                    connection.Open();
                    var reader = command.ExecuteReader();

                    if (reader != null && reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            var tradeType = reader.GetString(1);
                            var ticker = reader.GetString(2);
                            var shares = reader.GetInt32(3);
                            var limit = reader.GetDecimal(4);
                            var securityType = reader.GetString(5);
                            var orderDuration = reader.GetString(6);

                            var newResult = new LimitOrderDbResult(tradeType, ticker, shares, limit, securityType, orderDuration);
                            limitDBResults.Add(newResult);
                        }
                    }
                }
            }

            foreach (var result in limitDBResults)
            {
                var newSecurity = new Security();

                if (result.SecurityType == null)
                {
                    continue;                   
                }

                if (result.SecurityType == "Stock")
                {
                    newSecurity = new Stock("", result.Ticker, "", 0, 0);

                }
                else if (result.SecurityType == "Mutual Fund")
                {
                    newSecurity = new MutualFund("", result.Ticker, "", 0, 0);
                }

                var newTrade = new Trade(result.TradeType, newSecurity, result.Ticker, result.Shares, "Limit", result.Limit, result.OrderDuration);
                var newLimitOrder = new LimitOrder(newTrade);
                _myLimitOrders.Add(newLimitOrder);
            }
            
            //Send List<LimitOrder> to all startup listeners
            Messenger.Default.Send<LimitOrderMessage>(new LimitOrderMessage(_myLimitOrders, true));
        }      

        /// <summary>
        /// Adds a taxlot to an existing position in some security
        /// </summary>
        /// <param name="taxlotToAdd"></param>
        public void AddToPortfolioDatabase(Taxlot taxlotToAdd)
        {
            //Add to taxlot list
            _myTaxlots.Add(taxlotToAdd);

            //If position with ticker exists, add the taxlot to position.
            if (!_myPositions.Any(s => s.Ticker == taxlotToAdd.Ticker))
            {
                _myPositions.Add(new Position(taxlotToAdd, taxlotToAdd.SecurityType));
            }
            else
            {
                foreach (var pos in _myPositions.Where(s => s.Ticker == taxlotToAdd.Ticker))
                {
                    pos.Taxlots.Add(taxlotToAdd);
                }
            }            
        }

        /// <summary>
        /// Adds all tickers in the portfolio to the
        /// positions to be deleted from the database
        /// upon exit.
        /// </summary>
        public void DeletePortfolio(List<Position> positions)
        {
            DeletePositions(positions);
        }

        public List<Position> GetEmptyPositionsList()
        {
            return _myPositions;
        }

        private void SetLocalMode(LocalModeMessage message)
        {
            _localMode = message.LocalMode;
        }

        private void HandleBuy(TradeBuyMessage message)
        {
            AddToPortfolioDatabase(message.Taxlot);
        }

        private void HandleSell(TradeSellMessage message)
        {
            
        }

    }

}
