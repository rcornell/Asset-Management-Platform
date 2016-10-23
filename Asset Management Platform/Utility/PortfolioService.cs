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
    public class PortfolioService: IPortfolioService
    {
        private Dictionary<string, double> _positionValues;
        public Dictionary<string, double> PositionValues
        {
            get { return _positionValues; }
            set { _positionValues = value; }
        }


        private StockDataService _stockDataService;
        private DispatcherTimer _timer;
        private List<Security> _securityList;

        private Portfolio _myPortfolio;
        public Portfolio CurrentPortfolio {
                get {
                    CalculatePositionValues();
                    return _myPortfolio;
                    }
                set { _myPortfolio = value; }
            }

        public PortfolioService()
        {
            _stockDataService = SimpleIoc.Default.GetInstance<StockDataService>();
            _securityList = _stockDataService.LoadSecurityDatabase();
            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 10);
                       
            CalculatePositionValues();
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
            bool securityDatabaseUpdated = _stockDataService.UpdateSecurityDatabase();
            if (securityDatabaseUpdated)
            {
                _securityList = _stockDataService.SecurityList;
            }
            CalculatePositionValues();
        }

        public void CalculatePositionValues()
        {
            foreach (var pos in CurrentPortfolio.MyPortfolio)
            {
                var ticker = pos.Ticker;
                var security = _securityList.Find(s => s.Ticker == ticker);
                var value = security.LastPrice * pos.SharesOwned;
                _positionValues.Add(ticker, value);
            }

            //Add try catch when you know what kind of errors this can lead to.
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


    }
}
