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
        private List<Security> _securityDatabaseList; //Known securities. Not currently used for anything.

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

        private List<Taxlot> _portfolioTaxlots;
        private List<Position> _portfolioPositions;
        private List<Security> _portfolioSecurities;


        public PortfolioManagementService(IStockDataService stockDataService, IPortfolioDatabaseService portfolioDatabaseService)
        {
            _stockDataService = stockDataService;
            _portfolioDatabaseService = portfolioDatabaseService;

            //Load known security info from SQL DB
            _securityDatabaseList = _stockDataService.LoadSecurityDatabase();

            //Create a list of owned securities
            BuildPortfolioSecurities();

            //Build list of DisplaySecurities
            BuildDisplaySecurityLists();

            //Download limit orders from DB
            GetLimitOrderList();

            _timer = new DispatcherTimer();
            _timer.Tick += _timer_Tick;
            _timer.Interval = new TimeSpan(0, 0, 10);    
        }


        /// <summary>
        /// Creates the list of taxlots, positions, and securities owned.
        /// </summary>
        private void BuildPortfolioSecurities()
        {
            _portfolioTaxlots = _portfolioDatabaseService.GetTaxlotsFromDatabase();
            if (_portfolioTaxlots.Count > 0)
                _portfolioPositions = _portfolioDatabaseService.GetPositionsFromTaxlots(_portfolioTaxlots);
            else
                _portfolioPositions = new List<Position>();

            var tickers = new List<string>();
            foreach (var position in _portfolioPositions)
            {
                tickers.Add(position.Ticker);
            }

            //Could combine these into a startup GetAllInfo method in StockDataService
            var rawSecurities = _stockDataService.GetSecurityInfo(tickers);
            _portfolioSecurities = _stockDataService.GetMutualFundExtraData(rawSecurities);
        }


        /// <summary>
        /// Creates the lists of UI-bindable stocks and mutual funds.
        /// Separate lists need to be maintained if the user switches
        /// the UI display criteria.
        /// </summary>
        private void BuildDisplaySecurityLists()
        {
            _displayStocks = new List<DisplayStock>();
            _displayMutualFunds = new List<DisplayMutualFund>();

            foreach (var pos in _portfolioPositions)
            {
                //Search the list of OWNED securities for a match
                var matchingSecurity = _portfolioSecurities.Find(s => s.Ticker == pos.Ticker);

                //If no match within security list, look it up.
                if (matchingSecurity == null)
                {
                    matchingSecurity = _stockDataService.GetSecurityInfo(pos.Ticker);
                }

                //Create appropriate security type and add to lists for UI.
                if (matchingSecurity != null && matchingSecurity is Stock)
                    _displayStocks.Add(new DisplayStock(pos, (Stock)matchingSecurity));
                else if (matchingSecurity != null && matchingSecurity is MutualFund)
                    _displayMutualFunds.Add(new DisplayMutualFund(pos, (MutualFund)matchingSecurity));
            }
        }

        private void GetLimitOrderList()
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
            if (trade.Security is Stock && !_portfolioPositions.Any(s => s.Ticker == trade.Ticker))
            {
                //Add position to portfolio database for online storage
                var taxlot = new Taxlot(trade.Ticker, trade.Shares, trade.Security.LastPrice, DateTime.Now, trade.Security);
                var position = new Position(taxlot);
                _portfolioDatabaseService.AddToPortfolioDatabase(position);

                //Add new displaystock for UI
                DisplayStocks.Add(new DisplayStock(position, (Stock)trade.Security));
            }
            //Ticker exists in database and security is stock
            else if (trade.Security is Stock && _portfolioPositions.Any(s => s.Ticker == trade.Ticker))
            {
                //See if this affects the DisplayStock in the UI
                var taxlot = new Taxlot(trade.Ticker, trade.Shares, trade.Security.LastPrice, DateTime.Now, trade.Security);
                _portfolioDatabaseService.AddToPortfolioDatabase(taxlot);
            }
            //This ticker isn't already owned and it is a MutualFund
            else if (trade.Security is MutualFund && !_portfolioPositions.Any(s => s.Ticker == trade.Ticker))
            {
                var taxlot = new Taxlot(trade.Ticker, trade.Shares, trade.Security.LastPrice, DateTime.Now, trade.Security);
                var position = new Position(taxlot);
                _portfolioDatabaseService.AddToPortfolioDatabase(position);
                DisplayMutualFunds.Add(new DisplayMutualFund(position, (MutualFund)trade.Security));
            }
            else if (trade.Security is MutualFund && _portfolioPositions.Any(s => s.Ticker == trade.Ticker))
            {
                //Security is known and already held, so just add the new taxlot.
                var taxlot = new Taxlot(trade.Ticker, trade.Shares, trade.Security.LastPrice, DateTime.Now, trade.Security);
                _portfolioDatabaseService.AddToPortfolioDatabase(taxlot);
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
                SellPosition(trade);
            }
            else if (validOrder && trade.Terms == "Market")
            {
                //Order is valid and a market order
                SellPosition(trade);
            }
        }


        private void SellPosition(Trade trade)
        {
            var securityBeingSold = trade.Security;
            var ticker = trade.Ticker;
            var shares = trade.Shares;

            //Search owned positions for a match with the trade's ticker
            var position = _portfolioPositions.Find(p => p.Ticker == trade.Ticker);
            var securityType = position.GetSecurityType();

            //Use security type to direct execution to correct sale method
            if(securityType is Stock)
            {
                SellStock(trade, position);
            }
            else if (securityType is MutualFund)
            {
                SellMutualFund(trade, position);
            }  
        }


        private void SellMutualFund(Trade trade, Position position)
        {
            var security = trade.Security;
            var ticker = trade.Ticker;
            var shares = trade.Shares;

            if (shares == position.SharesOwned)
            {
                //Tell PortfolioDatabaseService to remove shares from DB







                //This is the question now
                //_portfolioDatabaseService.SellSharesFromPortfolioDatabase(security, shares);












                //Find and remove the security from portfolio
                var securityToRemove = _portfolioSecurities.Find(s => s.Ticker == ticker);
                _portfolioSecurities.Remove(securityToRemove);

                //Find and remove all taxlots
                var originalTaxLots = new List<Taxlot>(_portfolioTaxlots);
                var taxlotsToRemove = originalTaxLots.Where(t => t.Ticker == ticker);
                foreach (var lot in taxlotsToRemove)
                {
                    _portfolioTaxlots.Remove(lot);
                }

                //Remove the position that was passed in to this method
                _portfolioPositions.Remove(position);

                //Remove the DisplayMutualFund
                var displayFundToRemove = _displayMutualFunds.Find(f => f.Ticker == ticker);
                _displayMutualFunds.Remove(displayFundToRemove);
            }
            else if (shares > position.SharesOwned)
            {
                var message = new TradeMessage() { Shares = shares, Ticker = ticker, Message = "Order quantity exceeds shares owned!" };
                Messenger.Default.Send(message);
            }
            else //selling partial position
            {
                //Tell PortfolioDatabaseService to sell shares from the position
                //_portfolioDatabaseService.SellSharesFromPortfolioDatabase(security, shares);

                //The position reduces its shares
                //Does this flow to UI?
                position.SellShares(shares);
            }
        }

        private void SellStock(Trade trade, Position position)
        {
            var security = trade.Security;
            var ticker = trade.Ticker;
            var shares = trade.Shares;

            if (shares == position.SharesOwned)
            {
                //Tell PortfolioDatabaseService to remove shares from DB
                //_portfolioDatabaseService.SellSharesFromPortfolioDatabase(security, shares);

                //Find and remove the security from portfolio
                var securityToRemove = _portfolioSecurities.Find(s => s.Ticker == ticker);
                _portfolioSecurities.Remove(securityToRemove);

                //Find and remove all taxlots
                var originalTaxLots = new List<Taxlot>(_portfolioTaxlots);
                var taxlotsToRemove = originalTaxLots.Where(t => t.Ticker == ticker);
                foreach (var lot in taxlotsToRemove)
                {
                    _portfolioTaxlots.Remove(lot);
                }

                //Remove the position that was passed in to this method
                _portfolioPositions.Remove(position);

                //Remove the DisplayMutualFund
                var displayFundToRemove = _displayStocks.Find(f => f.Ticker == ticker);
                _displayStocks.Remove(displayFundToRemove);
            }
            else if (shares > position.SharesOwned)
            {
                var message = new TradeMessage() { Shares = shares, Ticker = ticker, Message = "Order quantity exceeds shares owned!" };
                Messenger.Default.Send(message);
            }
            else //selling partial position
            {
                //Tell PortfolioDatabaseService to sell shares from the position
                //_portfolioDatabaseService.SellSharesFromPortfolioDatabase(security, shares);

                //The position reduces its shares
                //Does this flow to UI?
                position.SellShares(shares);
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
                        SellPosition(newTrade);
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
            _displayMutualFunds.Clear();
            _portfolioSecurities.Clear();
            _portfolioTaxlots.Clear();
            _portfolioDatabaseService.DeletePortfolio(_portfolioPositions);
        }

        public void TestLimitOrderMethods()
        {
            UpdatePortfolioPrices();
            CheckLimitOrdersForActive();
        }

    }
}
