using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Messaging;
using Asset_Management_Platform.Messages;

namespace Asset_Management_Platform.Utility
{
    public class PortfolioManagementService : IPortfolioManagementService
    {
        private List<string> _tickers;
        private IStockDataService _stockDataService;
        private IPortfolioDatabaseService _portfolioDatabaseService;
        private DispatcherTimer _timer;
        private List<Security> _securityList; //used for pricing

        private List<DisplayStock> _displayStocks;
        public List<DisplayStock> DisplayStocks //used to display in MainViewModel
        {
            get
            {
                return _displayStocks;
            }
        }

        public PortfolioManagementService(IStockDataService stockDataService, IPortfolioDatabaseService portfolioDatabaseService)
        {
            _stockDataService = stockDataService;
            _portfolioDatabaseService = portfolioDatabaseService;

            _stockDataService.Initialize();

            //Load stock info from SQL DB
            _securityList = _stockDataService.LoadSecurityDatabase();

            //Use yahooAPI to pull in updated info
            var updateSuccessful = _stockDataService.UpdateSecurityDatabase();

            if (updateSuccessful) 
            {
                _tickers = GetTickers();
            }
            else
            {
                //Security list update failed.
                throw new NotImplementedException();
            }
            
            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 10);

            BuildDisplayStocks();
        }

        private void BuildDisplayStocks()
        {
            var positions = _portfolioDatabaseService.GetPositions();
            var stocks = _stockDataService.GetSecurityList();
            //this cannot find GME in the stock list...beacuse it's not in S&P. Need to refactor. The list at
            //startup may be pointless


            _displayStocks = new List<DisplayStock>();

            foreach (var pos in positions)
            {
                var stock = stocks.Find(s => s.Ticker == pos.Ticker);
                if (stock == null)
                    //shouldn't hit this once StockDataService is keeping database up to date
                    stock = _stockDataService.GetSpecificStockInfo(pos.Ticker);
                _displayStocks.Add(new DisplayStock(pos, (Stock)stock));
            } //check to see if the stocks are Stocks or Securities
        }

        /// <summary>
        /// Extracts the tickers from the list of securities
        /// so that the simple list can be sent to the yahooAPI
        /// without having to foreach throught the list and
        /// make a list of tickers every time.
        /// </summary>
        /// <returns></returns>
        private List<string> GetTickers()
        {
            var tickers = new List<string>();
            if (_securityList == null || _securityList.Count == 0)
                return tickers;

            foreach (var security in _securityList)
            {
                tickers.Add(security.Ticker);
            }
            return tickers;
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
                _securityList = _stockDataService.GetUpdatedPrices();
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

        public void AddPosition(Stock stock, string ticker, int shares)
        {


            //BUILD COST BASIS CORRECTLY

            if (stock != null && !string.IsNullOrEmpty(ticker) && shares > 0) {
                if (!_securityList.Contains(stock))
                {
                    _securityList.Add(stock);
                }

                if (!ticker.Contains(ticker))//wrong
                    _tickers.Add(ticker); //use boolean return for something?

                var taxlot = new Taxlot(ticker, shares, decimal.Parse(stock.LastPrice.ToString()), DateTime.Now);
                var position = new Position(taxlot);

                _portfolioDatabaseService.AddToPortfolio(position);
                _displayStocks.Add(new DisplayStock(position, stock)); //add a new DisplayStock bc of taxlot tracking

            }
        }

        public void SellPosition(Stock stock, string ticker, int shares)
        {
            if (stock != null && !string.IsNullOrEmpty(ticker) && shares > 0)
            {
                var displayStock = _displayStocks.Find(s => s.Ticker == ticker);
                if (shares == displayStock.Shares)
                {
                    _securityList.Remove(stock);
                    _tickers.Remove(ticker); //use boolean return for something?
                    _displayStocks.Remove(displayStock);
                }
                else if (shares > displayStock.Shares)
                {
                    var message = new TradeMessage() { Shares = shares, Ticker = ticker, Message = "Order quantity exceeds shares owned!" };
                    Messenger.Default.Send(message);
                }
                else //selling partial position
                {
                    displayStock.ReduceShares(shares);
                }
                

            }
        }

        public Stock GetOrderPreviewStock(string ticker)
        {
            var securityToReturn = _stockDataService.GetSpecificStockInfo(ticker);
            return securityToReturn;
        }

        public void UploadPortfolio()
        {
            _portfolioDatabaseService.SavePortfolioToDatabase();
        }

        public void DeletePortfolio()
        {
            _tickers.Clear();
            _displayStocks.Clear();
            _securityList.Clear();
            _portfolioDatabaseService.DeletePortfolio();
        }
    }
}
