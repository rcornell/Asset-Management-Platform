﻿using System;
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

namespace Asset_Management_Platform
{
    /// <summary>
    /// Manages the logic of the user's positions and taxlots
    /// </summary>
    public class PortfolioDatabaseService : IPortfolioDatabaseService
    {
        private List<string> _positionsToDelete;
        private SqlDataReader _reader;
        private List<Position> _portfolioOriginalState;
        private List<Position> _myPositions;
        private IStockDataService _stockDatabaseService;


        public PortfolioDatabaseService(IStockDataService stockDatabaseService)
        {
            _stockDatabaseService = stockDatabaseService;
            _positionsToDelete = new List<string>();
            if (!CheckDBForPositions())
                _myPositions = new List<Position>();
        }

        public List<Position> GetPositions()
        {
            return _myPositions;
        }

        /// <summary>
        /// Will attempt to load the MyPortfolio
        /// table from SQL Database. If no MyPortfolio
        /// table is found, it will return false.
        /// </summary>
        public bool CheckDBForPositions()
        {
            try
            {
                var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];
                using (var connection = new SqlConnection(storageString))
                {
                    connection.Open();
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = @"SELECT * FROM MyPortfolio;";
                        _reader = command.ExecuteReader();
                        if (_reader.HasRows == true)
                            return true;
                    }
                }
            }
            catch (SqlException ex)
            {
                var msg = new PortfolioMessage();
                msg.Message = ex.Message;
                Messenger.Default.Send(msg);
                return false;
            }
            catch (InvalidOperationException ex)
            {
                var msg = new PortfolioMessage();
                msg.Message = ex.Message;
                Messenger.Default.Send(msg);
                return false;
            }

            return false;
        }

        /// <summary>
        /// Creates a List<Position> 
        /// using the SQL Database's
        /// MyPortfolio table.
        /// </summary>
        //public void LoadPositionsFromDatabase()
        //{
        //    List<Taxlot> taxlotsFromDatabase = new List<Taxlot>();
        //    var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];
        //    using (var connection = new SqlConnection(storageString))
        //    {
        //        connection.Open();
        //        using (var command = new SqlCommand())
        //        {
        //            command.Connection = connection;
        //            command.CommandText = @"SELECT * FROM MyPortfolio;";
        //            var reader = command.ExecuteReader();
        //            while (reader.Read())
        //            {
        //                var ticker = reader.GetString(1);
        //                var quantity = int.Parse(reader.GetString(2));
        //                var purchasePrice = decimal.Parse(reader.GetString(3));
        //                DateTime datePurchased = new DateTime();
        //                if(reader.IsDBNull(4))
        //                    datePurchased = new DateTime(2000,12,31);
        //                else if (!string.IsNullOrEmpty(reader.GetString(4)))
        //                    datePurchased = DateTime.Parse(reader.GetString(4));

        //                var taxLot = new Taxlot(ticker, quantity, purchasePrice, datePurchased);
        //                taxlotsFromDatabase.Add(taxLot);
        //            }
        //        }
        //    }
        //    foreach (var lot in taxlotsFromDatabase)
        //    {
        //        if (!_myPositions.Any(s => s.Ticker == lot.Ticker))
        //        {
        //            _myPositions.Add(new Position(lot));
        //        }
        //        else if (_myPositions.Any(s => s.Ticker == lot.Ticker && s.SharesOwned != lot.Shares))
        //        {
        //            _myPositions.Find(s => s.Ticker == lot.Ticker).AddTaxlot(lot);
        //        }
        //        else if (_myPositions.Any(s => s.Ticker == lot.Ticker && s.SharesOwned == lot.Shares))
        //        {
        //            var pos = _myPositions.Find(s => s.Ticker == lot.Ticker);
        //            if (pos.Taxlots.Any(d => d.DatePurchased == lot.DatePurchased))
        //                continue;
        //            else
        //                pos.AddTaxlot(lot);
        //        }
        //    }

        //    foreach (var pos in _myPositions)
        //    {
        //        _databaseOriginalState.Add(new Position(pos.Taxlots));
        //    }
        //}

        public List<Taxlot> GetTaxlotsFromDatabase()
        {
            var taxlotsFromDatabase = new List<Taxlot>();
            var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];
            using (var connection = new SqlConnection(storageString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = @"SELECT * FROM MyPortfolio;";
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var ticker = reader.GetString(1);
                        var quantity = int.Parse(reader.GetString(2));
                        var purchasePrice = decimal.Parse(reader.GetString(3));
                        DateTime datePurchased = new DateTime();
                        if (reader.IsDBNull(4))
                            datePurchased = new DateTime(2000, 12, 31);
                        else if (!string.IsNullOrEmpty(reader.GetString(4)))
                            datePurchased = DateTime.Parse(reader.GetString(4));

                        var taxLot = new Taxlot(ticker, quantity, purchasePrice, datePurchased);
                        taxlotsFromDatabase.Add(taxLot);
                    }
                }
            }
            return taxlotsFromDatabase;
        }

        public List<Position> GetPositionsFromTaxlots(List<Taxlot> taxlots)
        {
            foreach (var lot in taxlots)
            {
                if (!_myPositions.Any(s => s.Ticker == lot.Ticker))
                {
                    _myPositions.Add(new Position(lot));
                }
                else if (_myPositions.Any(s => s.Ticker == lot.Ticker && s.SharesOwned != lot.Shares))
                {
                    _myPositions.Find(s => s.Ticker == lot.Ticker).AddTaxlot(lot);
                }
                else if (_myPositions.Any(s => s.Ticker == lot.Ticker && s.SharesOwned == lot.Shares))
                {
                    var pos = _myPositions.Find(s => s.Ticker == lot.Ticker);
                    if (pos.Taxlots.Any(d => d.DatePurchased == lot.DatePurchased))
                        continue;
                    else
                        pos.AddTaxlot(lot);
                }
            }


            foreach (var pos in _myPositions)
            {
                _portfolioOriginalState.Add(new Position(pos.Taxlots));
            }

            return _myPositions;

        }

        /// <summary>
        /// Compares _myPortfolio to the _databaseOriginalState from launch
        /// and creates lists of securities to update, insert, or delete.
        /// </summary>
        public void SavePortfolioToDatabase()
        {
            BackupDatabase();

            var positionsToInsert = new List<Position>();
            var positionsToUpdate = new List<Position>();

            foreach (var p in _myPositions)
            {
                
   
                if (_portfolioOriginalState.Any(pos => pos.Ticker == p.Ticker && pos.SharesOwned == p.SharesOwned))
                {
                    continue;
                }

                //Is the current position's ticker in the original state but the quantity is different?
                if (_portfolioOriginalState.Any(pos => pos.Ticker == p.Ticker && pos.SharesOwned != p.SharesOwned))
                {
                    positionsToUpdate.Add(p);
                }

                //Is the ticker not present in the original database?
                if (!_portfolioOriginalState.Any(pos => pos.Ticker == p.Ticker))
                {
                    positionsToInsert.Add(p);
                }

                //Is the quantity zero'd out from a sale?
                if (_portfolioOriginalState.Any(pos => pos.Ticker == p.Ticker && pos.SharesOwned == 0))
                {
                    _positionsToDelete.Add(p.Ticker);
                }
            }

            //If no inserts, updates, or deletes, exit method.
            if (positionsToInsert.Count == 0 && positionsToUpdate.Count == 0 && _positionsToDelete.Count == 0)
                return;

            try {

                var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];
                using (var connection = new SqlConnection(storageString))
                {
                    connection.Open();
                    using (var command = new SqlCommand())
                    {
                        command.Connection = connection;
                        //UPDATE POSITIONS IF NECESSARY
                        //May be unstable if it pushes too many commands too quickly
                        if (positionsToUpdate.Any()) { 
                            foreach (var pos in positionsToUpdate)
                            {
                                //Deletes all taxlots for position being updated
                                string deleteString = string.Format(@"DELETE FROM MyPortfolio WHERE Ticker='{0}';", pos.Ticker);     
                                command.CommandText = deleteString;
                                command.ExecuteNonQuery();
                            
                                //Re-adds all current taxlots
                                foreach (var lot in pos.Taxlots) {                                   
                                    command.CommandText = string.Format(@"INSERT INTO dbo.MyPortfolio (Ticker, Shares, CostBasis, DatePurchased) VALUES ('{0}' ,'{1}' ,'{2}' , '{3}');", lot.Ticker, lot.Shares, lot.PurchasePrice, lot.DatePurchased);
                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        //INSERT POSITIONS IF NECESSARY
                        if (positionsToInsert.Any())
                        {
                            string insertString = @"INSERT INTO dbo.MyPortfolio (Ticker, Shares, CostBasis, DatePurchased) VALUES ";

                            var finalPosition = positionsToInsert.Last();
                            var finalTaxlot = positionsToInsert.Last().Taxlots.Last();
                            foreach (var pos in positionsToInsert)
                            {
                                foreach (var lot in pos.Taxlots) { 
                                    //If the position being iterated is the last one, add the terminating SQL clause instead
                                    if (pos != finalPosition && lot != finalTaxlot)
                                    {
                                        insertString += string.Format("('{0}', '{1}', '{2}', '{3}'), ", lot.Ticker, lot.Shares, lot.PurchasePrice, lot.DatePurchased);
                                    }
                                    else
                                    {
                                        insertString += string.Format("('{0}', '{1}', '{2}', '{3}');", lot.Ticker, lot.Shares, lot.PurchasePrice, lot.DatePurchased);
                                    }
                                }
                            }
                            command.CommandText = insertString;
                            command.ExecuteNonQuery();
                        }

                        //DELETE POSITIONS IF NECESSARY
                        if (_positionsToDelete.Any())
                        {
                            string deleteString = @"DELETE FROM MyPortfolio WHERE Ticker =";

                            foreach (var pos in _positionsToDelete)
                            {
                                var deleteCommand = deleteString += string.Format(@" '{0}';", pos);
                                command.CommandText = deleteCommand;
                                command.ExecuteNonQuery();
                                //This will crash if more than one security is being deleted
                                //at a time.
                            }

                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                var msg = new DatabaseMessage(ex.Message, false);
                Messenger.Default.Send(msg);

            }
            catch (InvalidOperationException ex)
            {
                var msg = new DatabaseMessage(ex.Message, false);
                Messenger.Default.Send(msg);
            }
        }

        /// <summary>
        /// Creates a copy of MyPortfolio table in Database in case of update error.
        /// </summary>
        public void BackupDatabase()
        {
            var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];
            //Perhaps a way to create multiple backups?
            //string backup = @"SELECT * INTO MyPortfolioBackup FROM MyPortfolio;";
            string backup = @"INSERT INTO MyPortfolioBackup SELECT * FROM MyPortfolio";
            using (SqlConnection connection = new SqlConnection(storageString))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;

                    command.CommandText = @"TRUNCATE Table [MyPortfolioBackup];";
                    command.ExecuteNonQuery();

                    command.CommandText = backup;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UploadLimitOrdersToDatabase(List<LimitOrder> limitOrders)
        {
            var insertString = @"INSERT INTO dbo.MyLimitOrders (TradeType, Ticker, Shares, Limit, SecurityType, OrderDuration) VALUES ";
            var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];

            var final = limitOrders.Last();
            foreach (var order in limitOrders)
            {
                var tradeType = order.TradeType;
                var ticker = order.Ticker;
                var shares = order.Shares;
                var limit = order.Limit;
                var securityType = order.SecurityType.ToString();
                var orderDuration = order.OrderDuration;

                insertString += string.Format(@"('{0}', '{1}', {2}, {3}, '{4}', '{5}')", tradeType, ticker, shares, limit, securityType, orderDuration);
                if (order != final)
                    insertString += @", ";
            }

            try { 
                using (var connection = new SqlConnection(storageString))
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

                using (var connection = new SqlConnection(storageString))
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
                throw new NotImplementedException();
            }
        }

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
                default:
                    break;
            }

            var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];
            using (var truncateConnection = new SqlConnection(storageString)) {
                truncateConnection.Open();
                using (var truncateCommand = new SqlCommand(truncateString, truncateConnection))
                {
                    truncateCommand.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        /// Downloads and parses the list of limit orders from SQL table
        /// </summary>
        /// <returns></returns>
        public List<LimitOrder> LoadLimitOrdersFromDatabase()
        {
            var limitOrders = new List<LimitOrder>();
            var storageString = ConfigurationManager.AppSettings["StorageConnectionString"];
            var downloadString = @"SELECT * FROM dbo.MyLimitOrders;";
            SqlDataReader reader;
            var limitDBResults = new List<LimitOrderDBResult>();

            using (var connection = new SqlConnection(storageString))
            {
                using (var command = new SqlCommand(downloadString, connection))
                {
                    connection.Open();
                    reader = command.ExecuteReader();

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

                            var newResult = new LimitOrderDBResult(tradeType, ticker, shares, limit, securityType, orderDuration);
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
                    throw new NotImplementedException();
                }
                else if (result.SecurityType == "Stock")
                {
                    newSecurity = new Stock("", result.Ticker, "", 0, 0);

                }
                else if (result.SecurityType == "Mutual Fund")
                {
                    newSecurity = new MutualFund("", result.Ticker, "", 0, 0);
                }

                var newTrade = new Trade(result.TradeType, newSecurity, result.Ticker, result.Shares, "Limit", result.Limit, result.OrderDuration);
                var newLimitOrder = new LimitOrder(newTrade);
                limitOrders.Add(newLimitOrder);
            }

            return limitOrders;
        }      

        /// <summary>
        /// Adds a new security to a portfolio as a new Position
        /// with one taxlot
        /// </summary>
        /// <param name="positionToAdd"></param>
        public void AddToPortfolioDatabase(Position positionToAdd)
        {
            _myPositions.Add(positionToAdd);   
        }

        /// <summary>
        /// Adds a taxlot to an existing position in some security
        /// </summary>
        /// <param name="taxlotToAdd"></param>
        public void AddToPortfolioDatabase(Taxlot taxlotToAdd)
        {
            foreach(var pos in _myPositions.Where(s => s.Ticker == taxlotToAdd.Ticker)){
                pos.Taxlots.Add(taxlotToAdd);
            }
        }

        /// <summary>
        /// Takes a security and a share quantity. If the share quantity is equal to 
        /// the total position, the ticker is added to the list to be deleted
        /// when the database is updated.
        /// </summary>
        /// <param name="security"></param>
        /// <param name="shares"></param>
        public void SellSharesFromPortfolioDatabase(Security security, decimal shares)
        {
            foreach (var p in _myPositions.Where(p => p.Ticker == security.Ticker))
            {
                if (p.SharesOwned == shares)
                {
                    //var deleteThis = new Position(security.Ticker, shares);
                    _positionsToDelete.Add(p.Ticker);
                    break;
                }
                else
                {
                    p.SellShares(shares);
                }
            }
            //PROBABLY NEED TO SEND A MESSAGE TO UPDATE UI
        }

        /// <summary>
        /// Adds all tickers in the portfolio to the
        /// positions to be deleted from the database
        /// upon exit.
        /// </summary>
        public void DeletePortfolio()
        {
            foreach (var position in _myPositions)
            {
                _positionsToDelete.Add(position.Ticker);
            }
        }

    }

}
