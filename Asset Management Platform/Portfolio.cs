using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public class Portfolio
    {
        List<Position> positionsToDelete;

        private SqlDataReader reader;

        private List<Position> _databaseOriginalState;

        private List<Position> _myPortfolio;
        public List<Position> MyPortfolio
        {
            get { return _myPortfolio; }
            set { _myPortfolio = value; }
        }

        public Portfolio()
        {
             positionsToDelete = new List<Position>();
            _myPortfolio = new List<Position>();
        }


        /// <summary>
        /// Creates a List<Position> 
        /// using the SQL Database's
        /// MyPortfolio table.
        /// </summary>
        public void LoadPortfolioFromDatabase()
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
                        _myPortfolio.Add(new Position(ticker, quantity));
                    }
                }
            }

            //Does this work as intended?
            _databaseOriginalState = _myPortfolio;
        }

        /// <summary>
        /// Will attempt to load the MyPortfolio
        /// table from SQL Database. If no MyPortfolio
        /// table is found, it will return false.
        /// </summary>
        public bool CheckForPortfolio()
        {
            try {
                using (var connection = new SqlConnection("StorageConnectionString"))
                {
                    connection.Open();
                    using (var command = new SqlCommand())
                    {
                        command.CommandText = @"SELECT * FROM MYPORTFOLIO;";
                        reader = command.ExecuteReader();
                        if (reader.HasRows == true)
                            return true;
                    }
                }
            }
            catch (SqlException ex)
            {

                return false;
            }
            catch (InvalidOperationException ex)
            {
                return false;
            }

            return false;
        }

        
        private void SavePortfolioToDatabase()
        {
            var positionsToInsert = new List<Position>();
            var positionsToUpdate = new List<Position>();

            foreach (var p in _myPortfolio)
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

                }
            }

            //If no inserts, updates, or deletes, exit method.
            if (!positionsToInsert.Any() && !positionsToUpdate.Any() && !positionsToDelete.Any())
                return;

            try {
                using (var connection = new SqlConnection("StorageConnectionString"))
                {
                    connection.Open();
                    using (var command = new SqlCommand())
                    {
                        //May be unstable if it pushes too many commands too quickly
                        if (positionsToUpdate.Any()) { 
                            foreach (var pos in positionsToUpdate)
                            {
                                command.CommandText = string.Format("UPDATE MyPortfolio SET Quantity = {0} WHERE Ticker = {1}", pos.SharesOwned, pos.Ticker);
                                command.ExecuteNonQuery();
                            }
                        }

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

                        if (positionsToDelete.Any())
                        {
                            string deleteString = @"";
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                
            }
            catch (InvalidOperationException ex)
            {
  
            }
        }

        private void AddToPortfolio(Security securityToAdd, int shares)
        {
            var position = new Position(securityToAdd.Ticker, shares);
            _myPortfolio.Add(position);
            //PROBABLY NEED TO SEND A MESSAGE TO UPDATE UI
        }

        private void SellSharesFromPortfolio(Security security, int shares)
        {
            foreach (var p in _myPortfolio.Where(p => p.Ticker == security.Ticker))
            {
                if (p.SharesOwned == shares)
                {
                    var deleteThis = new Position(security.Ticker, shares);
                    _myPortfolio.Remove(new Position(security.Ticker, shares));
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
