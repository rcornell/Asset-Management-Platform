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
using Asset_Management_Platform.SecurityClasses;

namespace Asset_Management_Platform.Utility
{
    public class PortfolioManagementService : IPortfolioManagementService
    {
        private readonly DispatcherTimer _timer;        
        private bool _localMode;
        private List<Security> _securityDatabaseList;
        private List<LimitOrder> _limitOrderList;
        private List<Taxlot> _portfolioTaxlots;
        private List<Position> _portfolioPositions;
        private List<Security> _portfolioSecurities;
        private Security _requestedSecurity;
        private List<Security> _requestedSecurityList;

        public List<LimitOrder> LimitOrderList //used to display in MainViewModel
        {
            get
            {
                return _limitOrderList;
            }
        }
        
        public PortfolioManagementService()
        {
            //Register for LocalMode notification
            Messenger.Default.Register<LocalModeMessage>(this, SetLocalMode);

            //Register for IStockDataService creating its *database* of List<Security>
            Messenger.Default.Register<SecurityDatabaseMessage>(this, LoadSecurityDatabase);

            //Register for IPortfolioDatabaseService creating its List<Taxlot>
            Messenger.Default.Register<TaxlotMessage>(this, CreateTaxlots);

            Messenger.Default.Register<PositionMessage>(this, CreatePositions);

            //Register for IStockDataService returning Security/Securities information
            Messenger.Default.Register<StockDataResponseMessage>(this, HandleStockDataResponse);

            //Register for List<LimitOrder> creation
            Messenger.Default.Register<LimitOrderMessage>(this, CreateLimitOrders);

            //Register for handling of timer settings from UI
            Messenger.Default.Register<TimerMessage>(this, HandleTimerMessage);

            //Register method to handle trades from View
            Messenger.Default.Register<TradeMessage>(this, HandleTradeMessage);

            Messenger.Default.Register<LimitOrderUpdateResponseMessage>(this, HandleLimitOrderUpdateResponse);

            //Sends a message to update portfolio securities
            UpdatePortfolioSecuritiesStartup();

            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 10);
        }

        private void LoadSecurityDatabase(SecurityDatabaseMessage message)
        {
            _securityDatabaseList = message.SecurityList;
        }

        private void SetLocalMode(LocalModeMessage message)
        {
            _localMode = message.LocalMode;
        }

        private void CreateTaxlots(TaxlotMessage message)
        {
            _portfolioTaxlots = message.Taxlots;
        }

        private void CreatePositions(PositionMessage message)
        {
            if (message.IsStartup)
            {
                _portfolioPositions = message.Positions;
            }

        }

        private void HandleStockDataResponse(StockDataResponseMessage message)
        {
            if (message.IsStartupResponse && message.Securities != null)
            {
                _portfolioSecurities = message.Securities;
                return;
            }

            if (!message.IsStartupResponse && message.Security != null)
            {
                _requestedSecurity = message.Security;
                return;
            }

            if (!message.IsStartupResponse && message.Securities != null)
            {
                _requestedSecurityList = message.Securities;
            }
        }

        private void HandleTimerMessage(TimerMessage message)
        {
            _timer.Interval = message.Span;
            if (_timer.IsEnabled && message.StopTimer)
            {
                _timer.Stop();
            }
            if (!_timer.IsEnabled && message.StartTimer)
            {
                _timer.Start();
            }
        }


        /// <summary>
        /// Called when NOT in local mode.
        /// Creates the list of taxlots, positions, and securities owned.
        /// </summary>
        public async Task UpdatePortfolioSecuritiesStartup()
        {

            //NO MORE CALL TO PDS TO BUILD TAXLOTs

            //Get security data with market data API (currently YahooAPI)
            var tickers = _portfolioTaxlots.Select(s => s.Ticker).Distinct().ToList();
            Messenger.Default.Send<StockDataRequestMessage>(new StockDataRequestMessage(tickers, true));


            //YOU GOT THIS FAR WITH THE MESSAGES

            //Update all Positions' taxlot pricing
            foreach (var pos in _portfolioPositions)
            {
                var security = _portfolioSecurities.Find(s => s.Ticker == pos.Ticker);
                pos.UpdateTaxlotPrices(security.LastPrice);
            }

            Messenger.Default.Send(new DatabaseMessage("Complete", true, false));
        }

        /// <summary>
        /// This method is called in local mode when a user loads a
        /// stored json file with taxlot data
        /// </summary>
        /// <param name="taxlots"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePortfolioSecuritiesStartupLocal()
        {
            //Request updated pricing for portfolio positions
            Messenger.Default.Send<StockDataRequestMessage>(new StockDataRequestMessage(_portfolioPositions, false));

            Messenger.Default.Send(new DatabaseMessage("Success", true, false));
            return true;
        }

        private void HandleTradeMessage(TradeMessage message)
        {
            if (message.Trade.BuyOrSell == "Buy")
            { 
                Buy(message.Trade);
            }
            if (message.Trade.BuyOrSell == "Sell")
            { 
                Sell(message.Trade);
            }
        }

        /// <summary>
        /// Evaluate trade instructions and add to portfolio if appropriate.
        /// A limit order that is not active will be added to the list of 
        /// limit orders.
        /// </summary>
        /// <param name="trade"></param>
        public void Buy(Trade trade)
        {
            var limitType = false;

            //Check if any values are null or useless
            var validOrder = CheckOrderTerms(trade);
            var activeLimitOrder = CheckOrderLimit(trade);
            if (trade.Terms == "Limit" || trade.Terms == "Stop Limit" || trade.Terms == "Stop")
                limitType = true;

            if (validOrder && limitType && !activeLimitOrder)
            {
                //Order is valid but limit prevents execution
                CreateLimitOrder(trade);
                return;
            }

            if (validOrder && limitType)
            {
                //Order is valid and a limit-type and is active
                AddPosition(trade);
                return;
            }

            if (validOrder && trade.Terms == "Market")
            {
                //Order is valid and a market order
                AddPosition(trade);
            }
        }

        private void AddPosition(Trade trade)
        {
            if (!_securityDatabaseList.Any(s => s.Ticker == trade.Ticker))
                _securityDatabaseList.Add(trade.Security);

            //Check to confirm that shares of this security aren't already owned
            if (trade.Security is Stock && !_portfolioPositions.Any(s => s.Ticker == trade.Ticker))
            {
                //Create taxlot and position, then add to position list
                var taxlot = new Taxlot(trade.Ticker, trade.Shares, trade.Security.LastPrice, DateTime.Now, trade.Security, trade.Security.LastPrice);
                Messenger.Default.Send<TradeBuyMessage>(new TradeBuyMessage(trade, taxlot));
            }
            //Ticker exists in portfolio and security is stock
            else if (trade.Security is Stock && _portfolioPositions.Any(s => s.Ticker == trade.Ticker))
            {
                //Create new taxlot and add to existing position
                var taxlot = new Taxlot(trade.Ticker, trade.Shares, trade.Security.LastPrice, DateTime.Now, trade.Security, trade.Security.LastPrice);
                Messenger.Default.Send<TradeBuyMessage>(new TradeBuyMessage(trade, taxlot));
            }
            //Ticker is not already owned and is a MutualFund
            else if (trade.Security is MutualFund && !_portfolioPositions.Any(s => s.Ticker == trade.Ticker))
            {
                //Create new taxlot and add to existing position
                var taxlot = new Taxlot(trade.Ticker, trade.Shares, trade.Security.LastPrice, DateTime.Now, trade.Security, trade.Security.LastPrice);
                Messenger.Default.Send<TradeBuyMessage>(new TradeBuyMessage(trade, taxlot));
            }
            else if (trade.Security is MutualFund && _portfolioPositions.Any(s => s.Ticker == trade.Ticker))
            {
                //Create new taxlot and add to existing position
                var taxlot = new Taxlot(trade.Ticker, trade.Shares, trade.Security.LastPrice, DateTime.Now, trade.Security, trade.Security.LastPrice);
                Messenger.Default.Send<TradeBuyMessage>(new TradeBuyMessage(trade, taxlot));
            }
            
            //Sends updated List<Taxlot> and List<Position>
            Messenger.Default.Send<TradeCompleteMessage>(new TradeCompleteMessage(_portfolioPositions, _portfolioTaxlots, true));
        }

        /// <summary>
        /// Directs the executing code to the proper method for disposing
        /// of the security type. No real differences at the moment, but 
        /// there may be later.
        /// </summary>
        /// <param name="security"></param>
        /// <param name="ticker"></param>
        /// <param name="shares"></param>
        public void Sell(Trade trade)
        {
            var limitType = false;

            //Check if any values are null or useless
            var validOrder = CheckOrderTerms(trade);
            var isActiveLimitOrder = CheckOrderLimit(trade);
            if (trade.Terms == "Limit" || trade.Terms == "Stop Limit" || trade.Terms == "Stop")
                limitType = true;

            if (validOrder && limitType && !isActiveLimitOrder)
            {
                //Order is valid but limit prevents execution
                CreateLimitOrder(trade);
                return;
            }

            if (validOrder && limitType && isActiveLimitOrder)
            {
                //Order is valid and a limit type and is active
                SellPosition(trade);
                return;
            }

            if (validOrder && trade.Terms == "Market")
            {
                //Order is valid and a market order
                SellPosition(trade);
            }
        }

        private void SellPosition(Trade trade)
        {
            //Search owned positions for a match with the trade's ticker
            var position = _portfolioPositions.Find(p => p.Ticker == trade.Ticker);
            var securityType = position.GetSecurityType();
            var ticker = trade.Ticker;
            var shares = trade.Shares;
            
            if (shares == position.SharesOwned)
            {
                _portfolioSecurities.RemoveAll(s => s.Ticker == trade.Ticker);
                _portfolioTaxlots.RemoveAll(s => s.Ticker == trade.Ticker);
                _portfolioPositions.RemoveAll(s => s.Ticker == trade.Ticker);

                //Send message to PortfolioDatabaseService
                Messenger.Default.Send<TradeSellMessage>(new TradeSellMessage(trade, true, false));
            }            
            else if (shares > position.SharesOwned)
            {
                //User trying to sell too many shares
                var message = new TradeErrorMessage() { Shares = shares, Ticker = ticker, Message = "Order quantity exceeds shares owned!" };
                Messenger.Default.Send<TradeErrorMessage>(message);
            }
            else 
            {
                //User selling partial position
                position.SellShares(shares);

                //Send message to PortfolioDatabaseService
                Messenger.Default.Send<TradeSellMessage>(new TradeSellMessage(trade, false, true));
            }

            //Sends updated List<Taxlot> and List<Position>
            Messenger.Default.Send<TradeCompleteMessage>(new TradeCompleteMessage(_portfolioPositions, _portfolioTaxlots, true));
        }

        private void CreateLimitOrders(LimitOrderMessage message)
        {
            _limitOrderList = message.LimitOrders;
        }
        private void CreateLimitOrder(Trade trade)
        {
            var newLimitOrder = new LimitOrder(trade);

            if (_limitOrderList == null)
                _limitOrderList = new List<LimitOrder>();

            _limitOrderList.Add(newLimitOrder);

            //Send updated List<LimitOrder> to listeners in MainViewModel
            Messenger.Default.Send<LimitOrderMessage>(new LimitOrderMessage(_limitOrderList, false));
        }

        private bool CheckOrderTerms(Trade trade)
        {
            var security = trade.Security;
            var ticker = trade.Ticker;
            var shares = trade.Shares;
            var terms = trade.Terms;
            var limit = trade.Limit;
            var orderDuration = trade.OrderDuration;

            if (trade.Terms == "Limit" || trade.Terms == "Stop Limit" || trade.Terms == "Stop" || limit <= 0)
                return false;

            if (security != null && !string.IsNullOrEmpty(ticker) && shares > 0
                && !string.IsNullOrEmpty(terms) && !string.IsNullOrEmpty(orderDuration))
                return true;
            return false;
        }

        /// <summary>
        /// During trade execution, checks for a limit order and
        /// whether it is active or not.
        /// </summary>
        /// <param name="trade"></param>
        /// <returns></returns>
        private bool CheckOrderLimit(Trade trade)
        {
            var buyOrSell = trade.BuyOrSell;
            var terms = trade.Terms;
            var security = trade.Security;
            var limit = trade.Limit;

            //Buy Order validation
            if (buyOrSell == "Buy" && terms == "Limit" && security.LastPrice <= limit)
            {
                return true;
            }

            if (buyOrSell == "Buy" && terms == "Limit" && security.LastPrice >= limit)
            {
                return false;
            }

            //Sell Order validation
            if (buyOrSell == "Sell" && terms == "Limit" && security.LastPrice <= limit)
            {
                return true;
            }

            if (buyOrSell == "Sell" && terms == "Limit" && security.LastPrice >= limit)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Gets updated pricing for all tickers in the LimitOrderList,
        /// then compares the limits to the prices & Buy or Sell terms.
        /// If last price is valid vs. the limit, proceed with trade.
        /// </summary>
        private void CheckLimitOrdersForActive()
        {            
            var securitiesToCheck = new List<Security>();
            
            foreach (var order in LimitOrderList)
            {
                if(order.SecurityType is Stock)
                    securitiesToCheck.Add(new Stock("", order.Ticker, "", 0, 0));
                else if (order.SecurityType is MutualFund)
                    securitiesToCheck.Add(new MutualFund("", order.Ticker, "", 0, 0));
            }

            Messenger.Default.Send<LimitOrderUpdateRequestMessage>(new LimitOrderUpdateRequestMessage(
                securitiesToCheck, false));
        }

        private void HandleLimitOrderUpdateResponse(LimitOrderUpdateResponseMessage message)
        {
            var orderWasExecuted = false;
            var completedLimitOrders = new List<LimitOrder>();

            foreach (var sec in message.SecuritiesToCheck)
            {
                //Get all limit orders for the security being iterated
                var matches = LimitOrderList.Where(s => s.Ticker == sec.Ticker);

                foreach (var match in matches)
                {
                    var securityType = match.SecurityType;
                    var isActive = match.IsLimitOrderActive(sec.LastPrice);

                    if (isActive && match.TradeType == "Sell" && securityType is Stock)
                    {
                        var securityToTrade = new Stock("", sec.Ticker, sec.Description, sec.LastPrice, sec.Yield);
                        var newTrade = new Trade(match.TradeType, securityToTrade, match.Ticker, match.Shares, "Limit", match.Limit, match.OrderDuration);
                        SellPosition(newTrade);
                        completedLimitOrders.Add(match);
                        orderWasExecuted = true;
                    }
                    else if (isActive && match.TradeType == "Buy" && securityType is Stock)
                    {
                        var securityToTrade = new Stock("", sec.Ticker, sec.Description, sec.LastPrice, sec.Yield);
                        var newTrade = new Trade(match.TradeType, securityToTrade, match.Ticker, match.Shares, "Limit", match.Limit, match.OrderDuration);
                        AddPosition(newTrade);
                        completedLimitOrders.Add(match);
                        orderWasExecuted = true;
                    }
                    else if (isActive && match.TradeType == "Sell" && securityType is MutualFund)
                    {
                        var securityToTrade = new MutualFund("", sec.Ticker, sec.Description, sec.LastPrice, sec.Yield);
                        var newTrade = new Trade(match.TradeType, securityToTrade, match.Ticker, match.Shares, "Limit", match.Limit, match.OrderDuration);
                        SellPosition(newTrade);
                        completedLimitOrders.Add(match);
                        orderWasExecuted = true;
                    }
                    else if (isActive && match.TradeType == "Buy" && securityType is MutualFund)
                    {
                        var securityToTrade = new MutualFund("", sec.Ticker, sec.Description, sec.LastPrice, sec.Yield);
                        var newTrade = new Trade(match.TradeType, securityToTrade, match.Ticker, match.Shares, "Limit", match.Limit, match.OrderDuration);
                        AddPosition(newTrade);
                        completedLimitOrders.Add(match);
                        orderWasExecuted = true;
                    }
                }
            }

            foreach (var order in completedLimitOrders)
            {
                LimitOrderList.Remove(order);
            }

            if (orderWasExecuted)
            {
                Messenger.Default.Send<LimitOrderMessage>(new LimitOrderMessage(_limitOrderList, false));
            }
        }

        /// <summary>
        /// Will be called by the security screener
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public void GetTradePreviewSecurity(string ticker)
        {
            Messenger.Default.Send<StockDataRequestMessage>(new StockDataRequestMessage(ticker, false, true, false));
        }

        /// <summary>
        /// Returns PositionsByWeight for all securities
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<PositionByWeight> GetChartAllSecurities()
        {
            if (_portfolioPositions == null || !PositionsHaveValue())
                return new ObservableCollection<PositionByWeight>();

            decimal totalValue = 0;
            var positionsByWeight = new ObservableCollection<PositionByWeight>();

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

            return positionsByWeight;
        }

        /// <summary>
        /// Returns PositionsByWeight for stocks only
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<PositionByWeight> GetChartStocksOnly()
        {
            decimal totalValue = 0;
            var positionsByWeight = new ObservableCollection<PositionByWeight>();

            foreach (var pos in _portfolioPositions.Where(s => s.Security is Stock))
            {
                totalValue += pos.MarketValue;
            }

            foreach (var pos in _portfolioPositions.Where(s => s.Security is Stock))
            {
                var weight = (pos.MarketValue / totalValue) * 100;
                positionsByWeight.Add(new PositionByWeight(pos.Ticker, Math.Round(weight, 2)));
            }

            return positionsByWeight;
        }

        /// <summary>
        /// Returns PositionsByWeight for mutual funds only
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<PositionByWeight> GetChartFundsOnly()
        {
            decimal totalValue = 0;
            var positionsByWeight = new ObservableCollection<PositionByWeight>();

            foreach (var pos in _portfolioPositions.Where(s => s.Security is MutualFund))
            {
                totalValue += pos.MarketValue;
            }

            foreach (var pos in _portfolioPositions.Where(s => s.Security is MutualFund))
            {
                var weight = (pos.MarketValue / totalValue) * 100;
                positionsByWeight.Add(new PositionByWeight(pos.Ticker, Math.Round(weight, 2)));
            }

            return positionsByWeight;
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
            UpdatePortfolioPrices();
            CheckLimitOrdersForActive();
        }

        public void UpdateTimerInterval(TimeSpan timespan)
        {
            var clockIsRunning = _timer.IsEnabled;

            _timer.Stop();
            _timer.Interval = timespan;

            if (clockIsRunning)
                _timer.Start();
        }

        public void UpdatePortfolioPrices()
        {
            //Update Positions' pricing data
            Messenger.Default.Send<StockDataRequestMessage>(new StockDataRequestMessage(_portfolioPositions,false));
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

        public List<Position> GetPositions()
        {
            return _portfolioPositions;
        }

        public List<Taxlot> GetTaxlots()
        {
            return _portfolioTaxlots;
        }

        public List<LimitOrder> GetLimitOrders()
        {
            return LimitOrderList;
        }


        /// <summary>
        /// For a Sale, checks with list of user positions to make sure security types match
        /// For a buy, pulls the security info from StockDataService
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="tradeType"></param>
        /// <returns></returns>
        public async Task GetSecurityType(string ticker, string tradeType)
        {
            Messenger.Default.Send(new SecurityTypeRequestMessage(ticker));
        }       

        public void DeletePortfolio()
        {
            _portfolioSecurities.Clear();
            _portfolioTaxlots.Clear();
            //Delete portfolio message
        }

        /// <summary>
        /// For testing only.
        /// </summary>
        public void TestLimitOrderMethods()
        {
            UpdatePortfolioPrices();
            CheckLimitOrdersForActive();
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

        public bool IsLocalMode()
        {
            return _localMode;
        }
    }
}
