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

namespace Asset_Management_Platform.Utility
{
    public class PortfolioManagementService : IPortfolioManagementService
    {
        private IStockDataService _stockDataService;
        private IPortfolioDatabaseService _portfolioDatabaseService;
        private DispatcherTimer _timer;
        private List<Security> _securityDatabaseList; //Known securities, used for pricing

        private List<LimitOrder> _limitOrderList;
        public List<LimitOrder> LimitOrderList //used to display in MainViewModel
        {
            get
            {
                return _limitOrderList;
            }
        }

        private List<DisplayStock> _displayStocks;
        public List<DisplayStock> DisplayStocks //used to display in MainViewModel
        {
            get
            {
                return _displayStocks;
            }
        }

        private List<DisplayMutualFund> _displayMutualFunds;
        public List<DisplayMutualFund> DisplayMutualFunds //used to display in MainViewModel
        {
            get
            {
                return _displayMutualFunds;
            }
        }

        public PortfolioManagementService(IStockDataService stockDataService, IPortfolioDatabaseService portfolioDatabaseService)
        {
            _stockDataService = stockDataService;
            _portfolioDatabaseService = portfolioDatabaseService;
            _stockDataService.SeedDatabasesIfNeeded();

            //Load stock info from SQL DB
            _securityDatabaseList = _stockDataService.LoadSecurityDatabase();

            //Use yahooAPI to pull in updated info
            var updateSuccessful = _stockDataService.UpdateSecurityDatabase();

            //Build list of DisplaySecurities
            BuildDisplaySecurityLists();

            //Download limit orders from DB
            BuildLimitOrderList();


            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 10);    
        }


        /// <summary>
        /// Creates the lists of UI-bindable stocks and mutual funds.
        /// Separate lists need to be maintained if the user switches
        /// the selection criteria.
        /// </summary>
        private void BuildDisplaySecurityLists()
        {
            var positions = _portfolioDatabaseService.GetPositions();
            var securities = _stockDataService.GetSecurityList();

            _displayStocks = new List<DisplayStock>();
            _displayMutualFunds = new List<DisplayMutualFund>();

            foreach (var pos in positions)
            {
                //Search the known securities list for a match
                var matchingSecurity = securities.Find(s => s.Ticker == pos.Ticker);

                //If no match within security list, look it up.
                if (matchingSecurity == null) { 
                    matchingSecurity = _stockDataService.GetSecurityInfo(pos.Ticker);
                }

                //Create appropriate security type and add to lists for UI.
                if (matchingSecurity != null && matchingSecurity is Stock)
                    _displayStocks.Add(new DisplayStock(pos, (Stock)matchingSecurity));
                else if (matchingSecurity != null && matchingSecurity is MutualFund)
                    _displayMutualFunds.Add(new DisplayMutualFund(pos, (MutualFund)matchingSecurity));
            } 
        }

        private void BuildLimitOrderList()
        {
            _limitOrderList = _portfolioDatabaseService.LoadLimitOrdersFromDatabase();
        }

        public void Buy(Trade trade)
        {
            var limitType = false;

            //Check if any values are null or useless
            var validOrder = OrderTermsAreValid(trade);
            var isActiveLimitOrder = CheckOrderLimit(trade);
            if (trade.Terms == "Limit" || trade.Terms == "Stop Limit" || trade.Terms == "Stop")
                limitType = true;

            if (validOrder && limitType && !isActiveLimitOrder)
            {
                //Order is valid but limit prevents execution
                CreateLimitOrder(trade);
                return;
            }
            else if(validOrder && limitType && isActiveLimitOrder)
            {
                //Order is valid and a limit-type and is active
                AddPosition(trade);
                return;
            }
            else if (validOrder && trade.Terms == "Market")
            {
                //Order is valid and a market order
                AddPosition(trade);
            }
        }

        private void AddPosition(Trade trade)
        {
            if (!_securityDatabaseList.Any(s => s.Ticker == trade.Ticker))
                _securityDatabaseList.Add(trade.Security);

            //Check to confirm that shares of this security aren't in relevant
            //Stock or MutualFund list
            if (trade.Security is Stock && !DisplayStocks.Any(s => s.Ticker == trade.Ticker))
            {
                //Add position to portfolio database for online storage
                var taxlot = new Taxlot(trade.Ticker, trade.Shares, trade.Security.LastPrice, DateTime.Now);
                var position = new Position(taxlot);
                _portfolioDatabaseService.AddToPortfolio(position);

                //Add new displaystock for UI
                DisplayStocks.Add(new DisplayStock(position, (Stock)trade.Security));
            }
            //Ticker exists in database and security is stock
            else if (trade.Security is Stock && DisplayStocks.Any(s => s.Ticker == trade.Ticker))
            {
                //See if this affects the DisplayStock in the UI
                var taxlot = new Taxlot(trade.Ticker, trade.Shares, trade.Security.LastPrice, DateTime.Now);
                _portfolioDatabaseService.AddToPortfolio(taxlot);
            }
            //This ticker isn't already owned and it is a MutualFund
            else if (trade.Security is MutualFund && !DisplayMutualFunds.Any(s => s.Ticker == trade.Ticker))
            {

                var taxlot = new Taxlot(trade.Ticker, trade.Shares, trade.Security.LastPrice, DateTime.Now);
                var position = new Position(taxlot);
                _portfolioDatabaseService.AddToPortfolio(position);
                DisplayMutualFunds.Add(new DisplayMutualFund(position, (MutualFund)trade.Security));
            }
            else if (trade.Security is MutualFund && DisplayMutualFunds.Any(s => s.Ticker == trade.Ticker))
            {
                //Security is known and already held, so just add the new taxlot.
                var taxlot = new Taxlot(trade.Ticker, trade.Shares, trade.Security.LastPrice, DateTime.Now);
                _portfolioDatabaseService.AddToPortfolio(taxlot);
            }
        }


        private void CreateLimitOrder(Trade trade)
        {
            var newLimitOrder = new LimitOrder(trade);

            if (_limitOrderList == null)
                _limitOrderList = new List<LimitOrder>();

            _limitOrderList.Add(newLimitOrder);
        }

        private bool OrderTermsAreValid(Trade trade)
        {
            var security = trade.Security;
            var ticker = trade.Ticker;
            var shares = trade.Shares;
            var terms = trade.Terms;
            var limit = trade.Limit;
            var orderDuration = trade.OrderDuration;

            if (security != null && !string.IsNullOrEmpty(ticker) && shares > 0 
                && !string.IsNullOrEmpty(terms) && !string.IsNullOrEmpty(orderDuration))
                return true;
            return false;
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
            var validOrder = OrderTermsAreValid(trade);
            var isActiveLimitOrder = CheckOrderLimit(trade);
            if (trade.Terms == "Limit" || trade.Terms == "Stop Limit" || trade.Terms == "Stop")
                limitType = true;

            if (validOrder && limitType && !isActiveLimitOrder)
            {
                //Order is valid but limit prevents execution
                CreateLimitOrder(trade);
            }
            else if (validOrder && limitType && isActiveLimitOrder)
            {
                //Order is valid and a limit-type and is active
                if (trade.Security is Stock)
                    SellStock(trade);
                if (trade.Security is MutualFund)
                    SellMutualFund(trade);
            }
            else if (validOrder && trade.Terms == "Market")
            {
                //Order is valid and a market order
                if (trade.Security is Stock)
                    SellStock(trade);
                if (trade.Security is MutualFund)
                    SellMutualFund(trade);
            }
        }

        private void SellMutualFund(Trade trade)
        {
            var security = trade.Security;
            var ticker = trade.Ticker;
            var shares = trade.Shares;

            var displayMutualFund = _displayMutualFunds.Find(s => s.Ticker == ticker);

            if (shares == displayMutualFund.Shares)
            {
                _portfolioDatabaseService.SellSharesFromPortfolio(security, shares);
                _displayMutualFunds.Remove(displayMutualFund);
            }
            else if (shares > displayMutualFund.Shares)
            {
                var message = new TradeMessage() { Shares = shares, Ticker = ticker, Message = "Order quantity exceeds shares owned!" };
                Messenger.Default.Send(message);
            }
            else //selling partial position
            {
                _portfolioDatabaseService.SellSharesFromPortfolio(security, shares);
                displayMutualFund.ReduceShares(shares);
            }
            
        }

        private void SellStock(Trade trade)
        {
            var security = trade.Security;
            var ticker = trade.Ticker;
            var shares = trade.Shares;

            var displayStock = _displayStocks.Find(s => s.Ticker == ticker);

            if (shares == displayStock.Shares)
            {
                _portfolioDatabaseService.SellSharesFromPortfolio(security, shares);
                _displayStocks.Remove(displayStock);
            }
            else if (shares > displayStock.Shares)
            {
                var message = new TradeMessage() { Shares = shares, Ticker = ticker, Message = "Order quantity exceeds shares owned!" };
                Messenger.Default.Send(message);
            }
            else //selling partial position
            {
                _portfolioDatabaseService.SellSharesFromPortfolio(security, shares);
                displayStock.ReduceShares(shares);
            }
            
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
            else if (buyOrSell == "Buy" && terms == "Limit" && security.LastPrice >= limit)
            {
                return false;
            }

            //Sell Order validation
            if (buyOrSell == "Sell" && terms == "Limit" && security.LastPrice <= limit)
            {
                return true;
            }
            else if (buyOrSell == "Sell" && terms == "Limit" && security.LastPrice >= limit)
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
                if (order.SecurityType is MutualFund)
                    securitiesToCheck.Add(new MutualFund("", order.Ticker, "", 0, 0));
            }

            var updatedSecurities = _stockDataService.GetSecurityInfo(securitiesToCheck);

            foreach (var sec in updatedSecurities)
            {
                var matches = LimitOrderList.Where(s => s.Ticker == sec.Ticker);
                foreach (var match in matches)
                {
                    var isActive = match.IsLimitOrderActive(sec.LastPrice);
                    if (isActive && match.TradeType == "Sell")
                    {
                        var securityToTrade = new Security("", sec.Ticker, sec.Description, sec.LastPrice, sec.Yield);
                        var newTrade = new Trade(match.TradeType, securityToTrade, match.Ticker, match.Shares, "Limit", match.Limit, match.OrderDuration);
                        AddPosition(newTrade);
                    }
                    else if (isActive && match.TradeType == "Buy")
                    {
                        var securityToTrade = new Security("", sec.Ticker, sec.Description, sec.LastPrice, sec.Yield);
                        var newTrade = new Trade(match.TradeType, securityToTrade, match.Ticker, match.Shares, "Limit", match.Limit, match.OrderDuration);
                        Sell(newTrade);
                    }
                }
            }
        }

        /// <summary>
        /// Will be called by the security screener
        /// </summary>
        /// <param name="ticker"></param>
        /// <returns></returns>
        public Security GetOrderPreviewSecurity(string ticker)
        {
            var securityToReturn = _stockDataService.GetSecurityInfo(ticker);
            if (securityToReturn is Stock)
                return (Stock)securityToReturn;
            else if (securityToReturn is MutualFund)
                return (MutualFund)securityToReturn;
            else return new Stock("", "XXX", "Unknown Stock", 0, 0.00);
        }


        /// <summary>
        /// Will be called through the order entry system, where a security type
        /// must be selected to proceed
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="securityType"></param>
        /// <returns></returns>
        public Security GetOrderPreviewSecurity(string ticker, Security securityType)
        {
            var securityToReturn = _stockDataService.GetSecurityInfo(ticker, securityType);
            if (securityToReturn is Stock)
                return (Stock)securityToReturn;
            else if (securityToReturn is MutualFund)
                return (MutualFund)securityToReturn;
            else return new Stock("", "XXX", "Unknown Stock", 0, 0.00);
        }

        public ObservableCollection<PositionByWeight> GetChartAllSecurities()
        {
            decimal totalValue = 0;
            var positionsByWeight = new ObservableCollection<PositionByWeight>();

            foreach (var stock in _displayStocks)
            {
                totalValue += decimal.Parse(stock.MarketValue);
            }

            foreach (var fund in _displayMutualFunds)
            {
                totalValue += decimal.Parse(fund.MarketValue);
            }

            foreach (var stock in _displayStocks)
            {
                decimal weight = (decimal.Parse(stock.MarketValue) / totalValue) * 100;
                positionsByWeight.Add(new PositionByWeight(stock.Ticker, Math.Round(weight, 2)));
            }

            foreach (var fund in _displayMutualFunds)
            {
                decimal weight = (decimal.Parse(fund.MarketValue) / totalValue) * 100;
                positionsByWeight.Add(new PositionByWeight(fund.Ticker, Math.Round(weight, 2)));
            }

            return positionsByWeight;
        }

        public ObservableCollection<PositionByWeight> GetChartStocksOnly()
        {
            decimal totalValue = 0;
            var positionsByWeight = new ObservableCollection<PositionByWeight>();

            foreach (var stock in _displayStocks)
            {
                totalValue += decimal.Parse(stock.MarketValue);
            }

            foreach (var stock in _displayStocks)
            {
                decimal weight = (decimal.Parse(stock.MarketValue) / totalValue) * 100;
                positionsByWeight.Add(new PositionByWeight(stock.Ticker, Math.Round(weight, 2)));
            }

            return positionsByWeight;
        }

        public ObservableCollection<PositionByWeight> GetChartFundsOnly()
        {
            decimal totalValue = 0;
            var positionsByWeight = new ObservableCollection<PositionByWeight>();

            foreach (var fund in _displayMutualFunds)
            {
                totalValue += decimal.Parse(fund.MarketValue);
            }

            foreach (var fund in _displayMutualFunds)
            {
                decimal weight = (decimal.Parse(fund.MarketValue) / totalValue) * 100;
                positionsByWeight.Add(new PositionByWeight(fund.Ticker, Math.Round(weight, 2)));
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

        public void UpdatePortfolioPrices()
        {
            var listToUpdate = new List<Security>();            

            foreach (var stock in DisplayStocks)
            {
                listToUpdate.Add(stock.Stock);
            }

            foreach (var fund in DisplayMutualFunds)
            {
                listToUpdate.Add(fund.Fund);
            }

            var updatedSecurities = _stockDataService.GetSecurityInfo(listToUpdate);

            //Update all prices for any displaysecurity
            foreach (var security in updatedSecurities)
            {
                if (security is Stock) {
                    DisplayStocks.FindAll(s => s.Ticker == security.Ticker).ForEach(p => p.Stock.LastPrice = security.LastPrice);
                }
                if (security is MutualFund)
                {
                    DisplayMutualFunds.FindAll(m => m.Ticker == security.Ticker).ForEach(p => p.Fund.LastPrice = security.LastPrice);
                }
            }
            //Messenger.Default.Send(new PortfolioMessage());
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

        public List<DisplayMutualFund> GetDisplayMutualFunds()
        {
            return DisplayMutualFunds;
        }

        public List<LimitOrder> GetLimitOrders()
        {
            return LimitOrderList;
        }

        public void UploadAllToDatabase()
        {
            UploadPortfolio();
            UploadLimitOrdersToDatabase();
        }

        public void UploadLimitOrdersToDatabase()
        {
            _portfolioDatabaseService.UploadLimitOrdersToDatabase(LimitOrderList);
        }

        public void UploadPortfolio()
        {
            _portfolioDatabaseService.SavePortfolioToDatabase();
        }

        public void DeletePortfolio()
        {
            _displayStocks.Clear();
            _securityDatabaseList.Clear();
            _portfolioDatabaseService.DeletePortfolio();
        }

        public void TestLimitOrderMethods()
        {
            UpdatePortfolioPrices();
            CheckLimitOrdersForActive();
        }

    }
}
