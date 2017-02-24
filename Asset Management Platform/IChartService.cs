using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public interface IChartService
    {
        void GetChartAllSecurities();
        void GetChartStocksOnly();
        void GetChartFundsOnly();
    }
}
