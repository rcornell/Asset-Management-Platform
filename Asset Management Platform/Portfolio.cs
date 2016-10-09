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
        private SqlDataReader reader;

        private List<Position> _myPortfolio;
        public List<Position> MyPortfolio
        {
            get { return _myPortfolio; }
            set { _myPortfolio = value; }
        }

        public Portfolio()
        {
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

        
        private void SavePortfolio()
        {
            try { }
            catch { }
        }

        private bool AddToPortfolio(Security securityToAdd, int shares)
        {
            return true;
        }

        private bool SellSharesFromPortfolio(Security securityToRemove, int shares)
        {
            return true;
        }

        private bool RemoveFromPortfolio(Security securityToRemove)
        {
            return true;
        }
    }

}
