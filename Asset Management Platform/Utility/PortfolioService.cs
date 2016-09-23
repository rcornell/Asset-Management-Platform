using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using System.Windows.Threading;

namespace Asset_Management_Platform.Utility
{
    class PortfolioService
    {
        private StockDataService _stockValue;
        private DispatcherTimer _timer;
        private List<Security> _securityList;

        public PortfolioService(List<Security> securityList)
        {
            _stockValue = SimpleIoc.Default.GetInstance<StockDataService>();
            _securityList = securityList;
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 10);           
            CalculatePositionValues();
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
