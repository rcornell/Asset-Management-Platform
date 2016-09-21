using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;

namespace Asset_Management_Platform.Utility
{
    class PortfolioService
    {
        private StockValueService stockValue;

        public PortfolioService()
        {
            SimpleIoc.Default.Register<StockDataService>();
        }

    }
}
