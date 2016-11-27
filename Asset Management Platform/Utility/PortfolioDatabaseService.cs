using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System.Text;
using System.Threading.Tasks;
using Asset_Management_Platform.Messages;
using Asset_Management_Platform.Utility;
using System.Configuration;

namespace Asset_Management_Platform
{
    public class PortfolioDatabaseService : IPortfolioDatabaseService
    {
        private List<string> positionsToDelete;
        private SqlDataReader reader;
        private List<Position> _databaseOriginalState;

        private List<Position> _myPositions;

        public PortfolioDatabaseService()
        {
            _databaseOriginalState = new List<Position>();
             positionsToDelete = new List<string>();
            _myPositions = new List<Position>();
            if (CheckDBForPositions())
                LoadPositionsFromDatabase();
            else
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
                        reader = command.ExecuteReader();
                        if (reader.HasRows == true)
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
        public void LoadPositionsFromDatabase()
        {
            List<Taxlot> taxlotsFromDatabase = new List<Taxlot>();
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
                        var ticker = reader.GetString(0);
                        var quantity = int.Parse(reader.GetString(1));
                        var costBasis = decimal.Parse(reader.GetString(2));
                        DateTime datePurchased = new DateTime();
                        if(reader.IsDBNull(3))
                            datePurchased = new DateTime(2000,12,31);
                        else if (!string.IsNullOrEmpty(reader.GetString(3)))
                            datePurchased = DateTime.Parse(reader.GetString(3));

                        var taxLot = new Taxlot(ticker, quantity, costBasis, datePurchased);
                        taxlotsFromDatabase.Add(taxLot);
                    }
                }
            }

            foreach (var lot in taxlotsFromDatabase)
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
                _databaseOriginalState.Add(new Position(pos.Taxlots));
            }
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
                
   
                if (_databaseOriginalState.Any(pos => pos.Ticker == p.Ticker && pos.SharesOwned == p.SharesOwned))
                {
                    continue;
                }

                //Is the current position's ticker in the original state but the quantity is different?
                if (_databaseOriginalState.Any(pos => pos.Ticker == p.Ticker && pos.SharesOwned != p.SharesOwned))
                {
                    positionsToUpdate.Add(p);
                }

                //Is the ticker not present in the original database?
                if (!_databaseOriginalState.Any(pos => pos.Ticker == p.Ticker))
                {
                    positionsToInsert.Add(p);
                }

                //Is the quantity zero'd out from a sale?
                if (_databaseOriginalState.Any(pos => pos.Ticker == p.Ticker && pos.SharesOwned == 0))
                {
                    positionsToDelete.Add(p.Ticker);
                }
            }

            //If no inserts, updates, or deletes, exit method.
            if (positionsToInsert.Count == 0 && positionsToUpdate.Count == 0 && positionsToDelete.Count == 0)
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
                                command.CommandText = string.Format("UPDATE dbo.MyPortfolio SET Quantity = {0} WHERE Ticker = {1}", pos.SharesOwned, pos.Ticker);
                                command.ExecuteNonQuery();
                            }
                        }

                        //INSERT POSITIONS IF NECESSARY
                        if (positionsToInsert.Any())
                        {
                            string insertString = @"INSERT INTO dbo.MyPortfolio (Ticker, Shares, CostBasis) VALUES ";

                            var final = positionsToInsert.Last();
                            foreach (var pos in positionsToInsert)
                            {
                                //If the position being iterated is the last one, add the terminating SQL clause instead
                                if (pos != final)
                                {
                                    insertString += string.Format("('{0}', '{1}', '{2}'), ", pos.Ticker, pos.SharesOwned, pos.CostBasis);
                                }
                                else
                                {
                                    insertString += string.Format("('{0}', '{1}', '{2}');", pos.Ticker, pos.SharesOwned, pos.CostBasis);
                                }
                            }
                            command.CommandText = insertString;
                            command.ExecuteNonQuery();
                        }

                        //DELETE POSITIONS IF NECESSARY
                        if (positionsToDelete.Any())
                        {
                            string deleteString = @"DELETE FROM MyPortfolio WHERE Ticker =";

                            foreach (var pos in positionsToDelete)
                            {
                                var deleteCommand = deleteString += pos;
                                command.CommandText = deleteCommand;
                                command.BeginExecuteNonQuery();
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

        public void AddToPortfolio(Position positionToAdd)
        {
            _myPositions.Add(positionToAdd);   
        }

        /// <summary>
        /// Takes a security and a share quantity. If the share quantity is equal to 
        /// the total position, the ticker is added to the list to be deleted
        /// when the database is updated.
        /// </summary>
        /// <param name="security"></param>
        /// <param name="shares"></param>
        public void SellSharesFromPortfolio(Security security, int shares)
        {
            foreach (var p in _myPositions.Where(p => p.Ticker == security.Ticker))
            {
                if (p.SharesOwned == shares)
                {
                    _myPositions.Remove(p);
                    //var deleteThis = new Position(security.Ticker, shares);
                    positionsToDelete.Add(p.Ticker);
                }
                else
                {
                    p.SellShares(shares);
                }
            }
            //PROBABLY NEED TO SEND A MESSAGE TO UPDATE UI
        }


    }

}
