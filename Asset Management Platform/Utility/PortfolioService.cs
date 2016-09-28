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
        private StockDataService _stockValue;
        private DispatcherTimer _timer;
        private List<Security> _securityList;

        public PortfolioService(List<Security> securityList)
        {
            _stockValue = SimpleIoc.Default.GetInstance<StockDataService>();
            _securityList = securityList;
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
            if (_stockValue.UpdateDatabase())
            {
                _securityList = _stockValue.SecurityList;
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
