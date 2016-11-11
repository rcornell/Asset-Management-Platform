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
        private List<Security> _securityList;

        private List<DisplayStock> _displayStocks;
        public List<DisplayStock> DisplayStocks
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

            var displayStock = new List<DisplayStock>();

            foreach (var pos in positions)
            {
                var stock = stocks.Find(s => s.Ticker == pos.Ticker);
                displayStock.Add(new DisplayStock(pos, (Stock)stock));
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
    }
}
