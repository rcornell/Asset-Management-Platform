using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Asset_Management_Platform.Messages;
using System.Collections.ObjectModel;

namespace Asset_Management_Platform.Utility
{
    public class PortfolioManagementService : IPortfolioManagementService
    {
        private IStockDataService _stockDataService;
        private IPortfolioDatabaseService _portfolioDatabaseService;
        private DispatcherTimer _timer;
        private List<Security> _securityDatabaseList; //Known securities, used for pricing

        private List<DisplayStock> _displayStocks;
        public List<DisplayStock> DisplayStocks //used to display in MainViewModel
        {
            get
            {
                return _displayStocks;
            }
        }

        private List<DisplayMutualFund> _displayMutualFunds;
        public List<DisplayMutualFund> DisplayMutualFunds //used to display in MainViewModel
        {
            get
            {
                return _displayMutualFunds;
            }
        }

        public PortfolioManagementService(IStockDataService stockDataService, IPortfolioDatabaseService portfolioDatabaseService)
        {
            _stockDataService = stockDataService;
            _portfolioDatabaseService = portfolioDatabaseService;

            _stockDataService.Initialize();

            //Load stock info from SQL DB
            _securityDatabaseList = _stockDataService.LoadSecurityDatabase();

            //Use yahooAPI to pull in updated info
            var updateSuccessful = _stockDataService.UpdateSecurityDatabase();

            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 10);

            BuildDisplaySecurityLists();
        }


        private void BuildDisplaySecurityLists()
        {
            var positions = _portfolioDatabaseService.GetPositions();
            var securities = _stockDataService.GetSecurityList();

            _displayStocks = new List<DisplayStock>();
            _displayMutualFunds = new List<DisplayMutualFund>();

            foreach (var pos in positions)
            {
                //Search the known securities list for a match
                var matchingSecurity = securities.Find(s => s.Ticker == pos.Ticker);

                //If no match within security list, look it up.
                if (matchingSecurity == null) { 
                    matchingSecurity = _stockDataService.GetSpecificSecurityInfo(pos.Ticker);
                }

                //Create appropriate security type and add to lists for UI.
                if (matchingSecurity != null && matchingSecurity is Stock)
                    _displayStocks.Add(new DisplayStock(pos, (Stock)matchingSecurity));
                else if (matchingSecurity != null && matchingSecurity is MutualFund)
                    _displayMutualFunds.Add(new DisplayMutualFund(pos, (MutualFund)matchingSecurity));
            } 
        }


        /// <summary>
        /// When timer ticks, StockDataService uses YahooAPIService to update pricing 
        /// information for all securities in the list, then updates the security list
        /// in this class and sends out the PortfolioMessage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void _timer_Tick(object sender, EventArgs e)
        {
            bool updateSuccessful = _stockDataService.UpdateSecurityDatabase();
            if (updateSuccessful)
            {
                _securityDatabaseList = _stockDataService.GetUpdatedPrices();
            }
        }

        /// <summary>
        /// Starts the 10-second-interval update timer
        /// </summary>
        public void StartUpdates()
        {
            _timer.Start();
        }

        /// <summary>
        /// Stops the 10-second-interval update timer
        /// </summary>
        public void StopUpdates()
        {
            _timer.Stop();
        }

        public List<DisplayStock> GetDisplayStocks()
        {
            return DisplayStocks;
        }

        public List<DisplayMutualFund> GetDisplayMutualFunds()
        {
            return DisplayMutualFunds;
        }

        public void AddPosition(Security securityToAdd, string ticker, int shares, string terms, double limit, string orderDuration)
        {
            //Check if any values are null or useless
            var validOrder = OrderTermsAreValid(securityToAdd, ticker, shares, terms, limit, orderDuration);
            var isAwayFromLimit = CheckBuyOrderLimit(securityToAdd, terms, limit);

            if (validOrder && terms == "Limit" && isAwayFromLimit)
            {
                //Order is OK but limit prevents execution
                CreateLimitOrder(securityToAdd, ticker, shares, terms, limit, orderDuration);
                return;
            }
            else if (validOrder && terms == "Market")
            {
                if (!_securityDatabaseList.Any(s => s.Ticker == ticker))
                    _securityDatabaseList.Add(securityToAdd);

                //Check to confirm that shares of this security aren't in relevant
                //Stock or MutualFund list
                if (securityToAdd is Stock && !DisplayStocks.Any(s => s.Ticker == ticker))
                {
                    //Add position to portfolio database for online storage
                    var taxlot = new Taxlot(ticker, shares, securityToAdd.LastPrice, DateTime.Now);
                    var position = new Position(taxlot);
                    _portfolioDatabaseService.AddToPortfolio(position);

                    //Add new displaystock for UI
                    DisplayStocks.Add(new DisplayStock(position, (Stock)securityToAdd));
                }
                //Ticker exists in database and security is stock
                else if (securityToAdd is Stock && DisplayStocks.Any(s => s.Ticker == ticker))
                {
                    //See if this affects the DisplayStock in the UI
                    var taxlot = new Taxlot(ticker, shares, securityToAdd.LastPrice, DateTime.Now);
                    _portfolioDatabaseService.AddToPortfolio(taxlot);
                }
                //This ticker isn't already owned and it is a MutualFund
                else if (securityToAdd is MutualFund && !DisplayMutualFunds.Any(s => s.Ticker == ticker)){

                    var taxlot = new Taxlot(ticker, shares, securityToAdd.LastPrice, DateTime.Now);
                    var position = new Position(taxlot);
                    _portfolioDatabaseService.AddToPortfolio(position);
                    DisplayMutualFunds.Add(new DisplayMutualFund(position, (MutualFund)securityToAdd));
                }
                else if (securityToAdd is MutualFund && DisplayMutualFunds.Any(s => s.Ticker == ticker))
                {
                    //Security is known and already held, so just add the new taxlot.
                    var taxlot = new Taxlot(ticker, shares, securityToAdd.LastPrice, DateTime.Now);
                    _portfolioDatabaseService.AddToPortfolio(taxlot);
                }
            }
        }

        private void CreateLimitOrder(Security securityToAdd, string ticker, int shares, string terms, double limit, string orderDuration)
        {
            throw new NotImplementedException();
        }

        private bool OrderTermsAreValid(Security securityToAdd, string ticker, int shares, string terms, double limit, string orderDuration)
        {
            if (securityToAdd != null && !string.IsNullOrEmpty(ticker) && shares > 0 && !string.IsNullOrEmpty(terms) && !string.IsNullOrEmpty(orderDuration))
                return true;
            return false;
        }

        private bool CheckBuyOrderLimit(Security securityToAdd, string terms, double limit)
        {
            if (terms == "Limit" && securityToAdd.LastPrice <= (decimal)limit)
            {
                return false;
            }
            else if (terms == "Limit" && securityToAdd.LastPrice >= (decimal)limit)
                return true;

            return false;
        }

        /// <summary>
        /// Directs the executing code to the proper method for disposing
        /// of the security type. No real differences at the moment, but 
        /// there may be later.
        /// </summary>
        /// <param name="security"></param>
        /// <param name="ticker"></param>
        /// <param name="shares"></param>
        public void SellPosition(Security security, string ticker, int shares)
        {
            if (security is Stock)
                SellStock(security, ticker, shares);
            if (security is MutualFund)
                SellMutualFund(security, ticker, shares);     
        }

        private void SellMutualFund(Security security, string ticker, int shares)
        {
            if (security != null && !string.IsNullOrEmpty(ticker) && shares > 0){
                var displayMutualFund = _displayMutualFunds.Find(s => s.Ticker == ticker);

                if (shares == displayMutualFund.Shares)
                {
                    _portfolioDatabaseService.SellSharesFromPortfolio(security, shares);
                    _displayMutualFunds.Remove(displayMutualFund);
                }
                else if (shares > displayMutualFund.Shares)
                {
                    var message = new TradeMessage() { Shares = shares, Ticker = ticker, Message = "Order quantity exceeds shares owned!" };
                    Messenger.Default.Send(message);
                }
                else //selling partial position
                {
                    _portfolioDatabaseService.SellSharesFromPortfolio(security, shares);
                    displayMutualFund.ReduceShares(shares);
                }
            }
        }

        private void SellStock(Security security, string ticker, int shares)
        {
            if (security != null && !string.IsNullOrEmpty(ticker) && shares > 0)
            {
                var displayStock = _displayStocks.Find(s => s.Ticker == ticker);

                if (shares == displayStock.Shares)
                {
                    _portfolioDatabaseService.SellSharesFromPortfolio(security, shares);
                    _displayStocks.Remove(displayStock);
                }
                else if (shares > displayStock.Shares)
                {
                    var message = new TradeMessage() { Shares = shares, Ticker = ticker, Message = "Order quantity exceeds shares owned!" };
                    Messenger.Default.Send(message);
                }
                else //selling partial position
                {
                    _portfolioDatabaseService.SellSharesFromPortfolio(security, shares);
                    displayStock.ReduceShares(shares);
                }
            }
        }

        /// <summary>
        /// Will be called by the security screener
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public Security GetOrderPreviewSecurity(string ticker)
        {
            var securityToReturn = _stockDataService.GetSpecificSecurityInfo(ticker);
            if (securityToReturn is Stock)
                return (Stock)securityToReturn;
            else if (securityToReturn is MutualFund)
                return (MutualFund)securityToReturn;
            else return new Stock("", "XXX", "Unknown Stock", 0, 0.00);
        }


        /// <summary>
        /// Will be called through the order entry system, where a security type
        /// must be selected to proceed
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="securityType"></param>
        /// <returns></returns>
        public Security GetOrderPreviewSecurity(string ticker, Security securityType)
        {
            var securityToReturn = _stockDataService.GetSpecificSecurityInfo(ticker, securityType);
            if (securityToReturn is Stock)
                return (Stock)securityToReturn;
            else if (securityToReturn is MutualFund)
                return (MutualFund)securityToReturn;
            else return new Stock("", "XXX", "Unknown Stock", 0, 0.00);
        }

        public void UploadPortfolio()
        {
            _portfolioDatabaseService.SavePortfolioToDatabase();
        }

        public void DeletePortfolio()
        {
            _displayStocks.Clear();
            _securityDatabaseList.Clear();
            _portfolioDatabaseService.DeletePortfolio();
        }
    }
}
