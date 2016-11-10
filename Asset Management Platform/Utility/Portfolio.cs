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
    public class Portfolio : IPortfolio
    {
        private List<Position> positionsToDelete;
        private SqlDataReader reader;
        private List<Position> _databaseOriginalState;

        private List<Position> _myPositions;

        public Portfolio()
        {
             positionsToDelete = new List<Position>();
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
            using (var connection = new SqlConnection("StorageConnectionString"))
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.CommandText = @"SELECT * FROM MyPortfolio;";
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var ticker = reader.GetString(0);
                        var quantity = reader.GetInt32(1);
                        _myPositions.Add(new Position(ticker, quantity));
                    }
                }
            }

            //Does this work as intended?
            _databaseOriginalState = _myPositions;
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
                //Is current position in _myPortfolio unchanged from original state?
                if (_databaseOriginalState.Contains(p))
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
                    positionsToDelete.Add(p);
                }
            }

            //If no inserts, updates, or deletes, exit method.
            if (positionsToInsert.Count == 0 && positionsToUpdate.Count == 0 && positionsToDelete.Count == 0)
                return;

            try {
                using (var connection = new SqlConnection("StorageConnectionString"))
                {
                    connection.Open();
                    using (var command = new SqlCommand())
                    {
                        //UPDATE POSITIONS IF NECESSARY
                        //May be unstable if it pushes too many commands too quickly
                        if (positionsToUpdate.Any()) { 
                            foreach (var pos in positionsToUpdate)
                            {
                                command.CommandText = string.Format("UPDATE MyPortfolio SET Quantity = {0} WHERE Ticker = {1}", pos.SharesOwned, pos.Ticker);
                                command.ExecuteNonQuery();
                            }
                        }

                        //INSERT POSITIONS IF NECESSARY
                        if (positionsToInsert.Any())
                        {
                            string insertString = @"INSERT INTO MyPortfolio (Ticker, Quantity) VALUES";

                            var final = positionsToInsert.Last();
                            foreach (var pos in positionsToInsert)
                            {
                                //If the position being iterated is the last one, add the terminating SQL clause instead
                                if (pos != final)
                                {
                                    insertString += string.Format("({0}, {1}), ", pos.Ticker, pos.SharesOwned);
                                }
                                else
                                {
                                    insertString += string.Format("({0}, {1});", pos.Ticker, pos.SharesOwned);
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
                                var deleteCommand = deleteString += pos.Ticker;
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
            //Perhaps a way to create multiple backups?
            string backup = @"SELECT * FROM MyPortfolio INTO MyPortfolioBackup;";
            using (SqlConnection connection = new SqlConnection("StorageConnectionString"))
            {
                using (var command = new SqlCommand())
                {
                    command.CommandText = backup;
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddToPortfolio(Security securityToAdd, int shares)
        {
            var position = new Position(securityToAdd.Ticker, shares);
            _myPositions.Add(position);
            //PROBABLY NEED TO SEND A MESSAGE TO UPDATE UI
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
                    var deleteThis = new Position(security.Ticker, shares);
                    _myPositions.Remove(deleteThis);
                    positionsToDelete.Add(deleteThis);
                }
                else
                {
                    p.SharesOwned -= shares;
                }
            }
            //PROBABLY NEED TO SEND A MESSAGE TO UPDATE UI
        }
    }

}
