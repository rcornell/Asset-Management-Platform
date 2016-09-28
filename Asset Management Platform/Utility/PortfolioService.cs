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
    public class PortfolioService
    {
        private StockDataService _stockDataService;
        private DispatcherTimer _timer;
        private List<Security> _securityList;

        private Portfolio _myPortfolio;
        public Portfolio MyPortfolio {
                get {
                    CalculatePositionValues();
                    return _myPortfolio;
                    }
                set { _myPortfolio = value; }
            }

        public PortfolioService()
        {
            _stockDataService = SimpleIoc.Default.GetInstance<StockDataService>();
            _securityList = _stockDataService.LoadDatabase();
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
        private void _timer_Tick(object sender, EventArgs e)
        {
            if (_stockDataService.UpdateDatabase())
            {
                _securityList = _stockDataService.SecurityList;
                Messenger.Default.Send(new PortfolioMessage(_securityList));
            }
        }

        private void CalculatePositionValues()
        {
            throw new NotImplementedException();
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
