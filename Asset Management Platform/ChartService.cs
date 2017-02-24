using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Asset_Management_Platform.Messages;
using Asset_Management_Platform.SecurityClasses;

namespace Asset_Management_Platform
{
    public class ChartService : IChartService
    {
        private List<Position> _portfolioPositions;

        public ChartService()
        {
            Messenger.Default.Register<ChartRequestMessage>(this, HandleChartRequest);
        }

        private void HandleChartRequest(ChartRequestMessage message)
        {
            _portfolioPositions = message.Positions;

            if (message.ShowAll)
                GetChartAllSecurities();
            else if (message.ShowEquities)
                GetChartStocksOnly();
            else if (message.ShowMutualFunds)
                GetChartFundsOnly();
        }

        /// <summary>
        /// Returns PositionsByWeight for all securities
        /// </summary>
        /// <returns></returns>
        public void GetChartAllSecurities()
        {
            var positionsByWeight = new ObservableCollection<PositionByWeight>();
            decimal totalValue = 0;

            if (_portfolioPositions == null || !PositionsHaveValue())
                Messenger.Default.Send<ChartResponseMessage>(new ChartResponseMessage(positionsByWeight, true, false, false));                     

            foreach (var pos in _portfolioPositions)
            {
                //make this tolerate divide by zero
                totalValue += pos.MarketValue;
            }

            foreach (var pos in _portfolioPositions)
            {
                var weight = (pos.MarketValue / totalValue) * 100;
                positionsByWeight.Add(new PositionByWeight(pos.Ticker, Math.Round(weight, 2)));
            }

            Messenger.Default.Send<ChartResponseMessage>(new ChartResponseMessage(positionsByWeight, true, false, false));
        }

        /// <summary>
        /// Returns PositionsByWeight for stocks only
        /// </summary>
        /// <returns></returns>
        public void GetChartStocksOnly()
        {
            decimal totalValue = 0;
            var positionsByWeight = new ObservableCollection<PositionByWeight>();

            if (_portfolioPositions == null || !PositionsHaveValue())
                Messenger.Default.Send<ChartResponseMessage>(new ChartResponseMessage(positionsByWeight, false, true, false));

            foreach (var pos in _portfolioPositions.Where(s => s.Security is Stock))
            {
                totalValue += pos.MarketValue;
            }

            foreach (var pos in _portfolioPositions.Where(s => s.Security is Stock))
            {
                var weight = (pos.MarketValue / totalValue) * 100;
                positionsByWeight.Add(new PositionByWeight(pos.Ticker, Math.Round(weight, 2)));
            }

            Messenger.Default.Send<ChartResponseMessage>(new ChartResponseMessage(positionsByWeight, false, true, false));
        }

        /// <summary>
        /// Returns PositionsByWeight for mutual funds only
        /// </summary>
        /// <returns></returns>
        public void GetChartFundsOnly()
        {
            decimal totalValue = 0;
            var positionsByWeight = new ObservableCollection<PositionByWeight>();

            if (_portfolioPositions == null || !PositionsHaveValue())
                Messenger.Default.Send<ChartResponseMessage>(new ChartResponseMessage(positionsByWeight, false, false, true));

            foreach (var pos in _portfolioPositions.Where(s => s.Security is MutualFund))
            {
                totalValue += pos.MarketValue;
            }

            foreach (var pos in _portfolioPositions.Where(s => s.Security is MutualFund))
            {
                var weight = (pos.MarketValue / totalValue) * 100;
                positionsByWeight.Add(new PositionByWeight(pos.Ticker, Math.Round(weight, 2)));
            }

            Messenger.Default.Send<ChartResponseMessage>(new ChartResponseMessage(positionsByWeight, false, false, true));
        }

        private bool PositionsHaveValue()
        {
            foreach (var pos in _portfolioPositions)
            {
                if (pos.MarketValue > 0)
                    return true;
            }
            return false;
        }

    }
}
