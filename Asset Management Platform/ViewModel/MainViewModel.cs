using Asset_Management_Platform.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Asset_Management_Platform.Messages;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Asset_Management_Platform.SecurityClasses;
using System.Threading.Tasks;
using System.Configuration;
using Asset_Management_Platform.SaveLoad;

namespace Asset_Management_Platform
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// 

        #region All Fields
        private bool _limitOrdersHidden;
        private string _showLimitButtonText;
        private string _previewButtonText;
        private bool _orderTermsOK;
        private Security _screenerStock;
        private string _stockScreenerTicker;
        private bool _alertBoxVisible;
        private string _alertBoxMessage;
        private bool _executeButtonEnabled;
        private bool _limitBoxActive;
        private bool _limitOrderIsSelected;
        private ObservableCollection<string> _tradeDurationStrings;
        private ObservableCollection<string> _tradeTermStrings;
        private ObservableCollection<string> _tradeTypeStrings;
        private ObservableCollection<Security> _securityTypeStrings;
        private ObservableCollection<LimitOrder> _limitOrderList;
        private LimitOrder _selectedLimitOrder;
        private string _chartSubtitle;
        private string _previewDescription;
        private string _previewVolume;
        private string _previewAsk;
        private string _previewAskSize;
        private string _previewBid;
        private string _previewBidSize;
        private decimal _limitPrice;
        private Security _selectedSecurityType;
        private string _selectedDurationType;
        private string _selectedTradeType;
        private string _selectedTermType;
        private string _orderTickerText;
        private Security _previewSecurity;
        private ObservableCollection<PositionByWeight> _allocationChartPositions;
        public decimal _totalGainLoss;
        private decimal _previewPrice;
        private int _orderShareQuantity;
        private Position _selectedPosition;
        public decimal _totalValue;
        public decimal _totalCostBasis;
        private IPortfolioManagementService _portfolioManagementService;
        private ObservableCollection<Position> _positions;
        private ObservableCollection<Taxlot> _taxlots;
        private bool _showStocksOnly;
        private bool _showFundsOnly;
        private bool _showAllPositions;
        private List<Position> _hiddenPositions;
        private bool _previewOrderIsBusy;
        private bool _canSave;
        private bool _canLoad;
        private bool _localMode;
        private Security _requestedSecurity;
        private List<Security> _requestedSecurityList;
        #endregion

        #region All Properties
        public bool LimitOrdersHidden
        {
            get { return _limitOrdersHidden; }
            set
            {
                _limitOrdersHidden = value;
                RaisePropertyChanged(() => LimitOrdersHidden);
            }
        }       
        public string ShowLimitButtonText
        {
            get { return _showLimitButtonText; }
            set
            {
                _showLimitButtonText = value;
                RaisePropertyChanged(() => ShowLimitButtonText);
            }
        }    
        public string PreviewButtonText
        {
            get { return _previewButtonText; }
            set
            {
                _previewButtonText = value;
                RaisePropertyChanged(() => PreviewButtonText);
            }
        }       
        public bool OrderTermsOK
        {
            get { return _orderTermsOK; }
            set
            {
                _orderTermsOK = value;
                RaisePropertyChanged(() => OrderTermsOK);
            }
        }
        public Security ScreenerStock
        {
            get { return _screenerStock; }
            set { _screenerStock = value;
                RaisePropertyChanged(() => ScreenerStock);
            }
        }
        public string StockScreenerTicker
        {
            get { return _stockScreenerTicker; }
            set
            {
                _stockScreenerTicker = value.ToUpper();
                RaisePropertyChanged(() => StockScreenerTicker);
                ExecuteScreenerPreview(_stockScreenerTicker);
            }
        }
        public bool AlertBoxVisible {
            get { return _alertBoxVisible; }
            set { _alertBoxVisible = value;
                RaisePropertyChanged(() => AlertBoxVisible); }
        }      
        public string AlertBoxMessage {
            get { return _alertBoxMessage; }
            set { _alertBoxMessage = value;
                RaisePropertyChanged(() => AlertBoxMessage);
            }
        }
        public bool ExecuteButtonEnabled
        {
            get { return _executeButtonEnabled; }
            set
            {
                _executeButtonEnabled = value;
                RaisePropertyChanged(() => ExecuteButtonEnabled);
            }
        }
        public bool LimitBoxActive
        {
            get { return _limitBoxActive; }
            set { _limitBoxActive = value;
                RaisePropertyChanged(() => LimitBoxActive);
            }
        }
        public bool LimitOrderIsSelected
        {
            get { return _limitOrderIsSelected; }
            set
            {
                _limitOrderIsSelected = value;
                RaisePropertyChanged(() => LimitOrderIsSelected);
            }
        }
        public ObservableCollection<string> TradeDurationStrings
        {
            get { return _tradeDurationStrings; }
            set
            {
                _tradeDurationStrings = value;
                RaisePropertyChanged(() => TradeDurationStrings);
            }
        }
        public ObservableCollection<string> TradeTermStrings
        {
            get { return _tradeTermStrings; }
            set
            {
                _tradeTermStrings = value;
                RaisePropertyChanged(() => TradeTermStrings);
            }
        }
        public ObservableCollection<string> TradeTypeStrings
        {
            get { return _tradeTypeStrings; }
            set
            {
                _tradeTypeStrings = value;
                RaisePropertyChanged(() => TradeTypeStrings);
            }
        }
        public ObservableCollection<Security> SecurityTypes
        {
            get { return _securityTypeStrings; }
            set
            {
                _securityTypeStrings = value;
                RaisePropertyChanged(() => SecurityTypes);
            }
        }
        public ObservableCollection<LimitOrder> LimitOrderList
        {
            get { return _limitOrderList; }
            set
            {
                _limitOrderList = value;
                RaisePropertyChanged(() => LimitOrderList);
            }
        }
        public LimitOrder SelectedLimitOrder
        {
            get
            {
                return _selectedLimitOrder;
            }
            set
            {
                _selectedLimitOrder = value;
                if (_selectedLimitOrder != null)
                    LimitOrderIsSelected = true;
                else
                    LimitOrderIsSelected = false;
                RaisePropertyChanged(() => SelectedLimitOrder);
            }
        }
        public string ChartSubtitle {
            get { return _chartSubtitle; }
            set { _chartSubtitle = value;
                RaisePropertyChanged(() => ChartSubtitle);
            }
        }
        public string PreviewDescription {
            get { return _previewDescription; }
            set { _previewDescription = value;
                RaisePropertyChanged(() => PreviewDescription);
            }
        }
        public string PreviewVolume
        {
            get { return _previewVolume; }
            set
            {
                _previewVolume = value;
                RaisePropertyChanged(() => PreviewVolume);
            }
        }
        public string PreviewAsk
        {
            get { return _previewAsk; }
            set
            {
                _previewAsk = value;
                RaisePropertyChanged(() => PreviewAsk);
            }
        }
        public string PreviewAskSize
        {
            get { return _previewAskSize; }
            set
            {
                _previewAskSize = value;
                RaisePropertyChanged(() => PreviewAskSize);
            }
        }
        public string PreviewBid
        {
            get { return _previewBid; }
            set
            {
                _previewBid = value;
                RaisePropertyChanged(() => PreviewBid);
            }
        }
        public string PreviewBidSize
        {
            get { return _previewBidSize; }
            set
            {
                _previewBidSize = value;
                RaisePropertyChanged(() => PreviewBidSize);
            }
        }
        public decimal LimitPrice {
            get { return _limitPrice; }
            set { _limitPrice = value;
                _executeButtonEnabled = false;
                RaisePropertyChanged(() => LimitPrice);
                RevertExecuteButtonAndAlert();
            }
        }
        public Security SelectedSecurityType
        {
            get { return _selectedSecurityType; }
            set
            {
                _selectedSecurityType = value;
                RaisePropertyChanged(() => SelectedSecurityType);
                RevertExecuteButtonAndAlert();
            }
        }
        public string SelectedDurationType {
            get { return _selectedDurationType; }
            set
            {
                _selectedDurationType = value;
                RaisePropertyChanged(() => SelectedDurationType);
                RevertExecuteButtonAndAlert();
            }
        }
        public string SelectedTermType
        {
            get { return _selectedTermType; }
            set { _selectedTermType = value;
                RaisePropertyChanged(() => SelectedTermType);
                if (_selectedTermType == "Limit" || _selectedTermType == "Stop Limit" || _selectedTermType == "Stop") {
                    LimitBoxActive = true;                    
                }
                else
                {
                    LimitBoxActive = false;                    
                }
                RevertExecuteButtonAndAlert();
            }
        } 
        public string SelectedTradeType
        {
            get { return _selectedTradeType; }
            set { _selectedTradeType = value;
                RaisePropertyChanged(() => SelectedTradeType);
                RevertExecuteButtonAndAlert();
            }
        }
        public string OrderTickerText
        {
            get { return _orderTickerText; }
            set { _orderTickerText = value.ToUpper();
                RaisePropertyChanged(() => OrderTickerText);
                RevertExecuteButtonAndAlert();
            }
        }
        public int OrderShareQuantity
        {
            get { return _orderShareQuantity; }
            set { _orderShareQuantity = value;
                RaisePropertyChanged(() => OrderShareQuantity);
                RevertExecuteButtonAndAlert();
            }
        }       
        public decimal PreviewPrice
        {
            get
            {
                return _previewPrice;
            }
            set
            {
                _previewPrice = value;
                RaisePropertyChanged(() => PreviewPrice);
                RaisePropertyChanged(() => PreviewValue);
            }
        }
        public decimal PreviewValue
        {
            get { return (PreviewPrice * OrderShareQuantity); }
        }     
        public Position SelectedPosition
        {
            get { return _selectedPosition; }
            set {
                _selectedPosition = value;
                RaisePropertyChanged(() => SelectedPosition);
                if (_selectedPosition != null)
                {
                    ExecuteScreenerPreview(_selectedPosition.Ticker);
                    PopulateSelectedSecurityTerms();
                }
            }
        }
        public decimal TotalValue
        {
            get { return _totalValue; }
            set
            {
                _totalValue = value;
                RaisePropertyChanged(() => TotalValue);
            }
        }
        public decimal TotalCostBasis
        {
            get { return _totalCostBasis; }
            set
            {
                _totalCostBasis = value;
                RaisePropertyChanged(() => TotalCostBasis);
            }
        }
        public decimal TotalGainLoss
        {
            get { return _totalGainLoss; }
            set
            {
                _totalGainLoss = value;
                RaisePropertyChanged(() => TotalGainLoss);
            }
        }
        public ObservableCollection<PositionByWeight> AllocationChartPositions
        {
            get
            {
                return _allocationChartPositions;
            }
            set
            {
                _allocationChartPositions = value;
                RaisePropertyChanged(() => AllocationChartPositions);
            }
        }
        public Security PreviewSecurity
        {
            get { return _previewSecurity; }
            set
            {
                _previewSecurity = value;
                RaisePropertyChanged(() => PreviewSecurity);
            }

        }
        public ObservableCollection<Taxlot> Taxlots
        {
            get
            {
                return _taxlots;
            }
            set
            {
                _taxlots = value;
                RaisePropertyChanged(() => Taxlots);
            }
        }
        public ObservableCollection<Position> Positions
        {
            get
            {
                return _positions;
            }
            set
            {
                _positions = value;
                RaisePropertyChanged(() => Positions);
            }
        }

        #endregion

        #region All Commands

        public RelayCommand ToggleLimitDatagrid
        {
            get { return new RelayCommand(ExecuteToggleLimitDatagrid); }
        }

        public RelayCommand DeletePortfolio
        {
            get { return new RelayCommand(ExecuteDeletePortfolio); }
        }
        public RelayCommand SavePortfolio
        {
            get { return new RelayCommand(async () => await ExecuteSavePortfolio(), CanSave); }
        }
        public RelayCommand LoadPortfolio
        {
            get { return new RelayCommand(async () => await ExecuteLoadPortfolio(), CanLoad); }
        }

        public RelayCommand CloseApplication
        {
            get { return new RelayCommand(async () => await ExecuteCloseApplication(), CanSave); }
        }

        public RelayCommand PreviewOrder
        {
            get { return new RelayCommand(async () => await ExecutePreviewOrder(), CanPreview); }
        }

        public RelayCommand ExecuteOrder
        {
            get { return new RelayCommand(ExecuteExecuteOrder); }
        }

        public RelayCommand UpdatePrices
        {
            get { return new RelayCommand(ExecuteUpdatePrices); }
        }

        public RelayCommand ShowAllSecurities
        {
            get { return new RelayCommand(ExecuteShowAllSecurities); }
        }

        public RelayCommand ShowStocksOnly
        {
            get { return new RelayCommand(ExecuteShowStocksOnly); }
        }

        public RelayCommand ShowFundsOnly
        {
            get { return new RelayCommand(ExecuteShowFundsOnly); }
        }

        public RelayCommand DeleteLimitOrder
        {
            get { return new RelayCommand(ExecuteDeleteLimitOrder); }
        }

        public RelayCommand SetIntervalTenSeconds
        {
            get { return new RelayCommand(ExecuteSetIntervalTenSeconds); }
        }

        public RelayCommand SetIntervalThirtySeconds
        {
            get { return new RelayCommand(ExecuteSetIntervalThirtySeconds); }
        }        

        public RelayCommand SetIntervalSixtySeconds
        {
            get { return new RelayCommand(ExecuteSetIntervalSixtySeconds); }
        }

        public RelayCommand SetIntervalFiveMinutes
        {
            get { return new RelayCommand(ExecuteSetIntervalFiveMinutes); }
        }
        #endregion

        public MainViewModel(IPortfolioManagementService portfolioService)
        {
            Messenger.Default.Register<LocalModeMessage>(this, SetLocalMode);
            Messenger.Default.Register<TaxlotMessage>(this, CreateTaxlots);
            Messenger.Default.Register<PositionMessage>(this, CreatePositions);
            Messenger.Default.Register<TradeCompleteMessage>(this, ProcessTradeComplete);
            Messenger.Default.Register<LimitOrderMessage>(this, ProcessLimitOrderCreated);
            Messenger.Default.Register<StockDataResponseMessage>(this, ProcessStockDataResponse);
            _portfolioManagementService = portfolioService;

            TradeTypeStrings = new ObservableCollection<string>() { " ", "Buy", "Sell" };
            TradeTermStrings = new ObservableCollection<string>() { " ", "Market", "Limit", "Stop", "Stop Limit" };
            TradeDurationStrings = new ObservableCollection<string> { " ", "Day", "GTC", "Market Close", "Market Open", "Overnight" };
            SecurityTypes = new ObservableCollection<Security> { new Stock(), new MutualFund() };

            Positions = new ObservableCollection<Position>();
            Taxlots = new ObservableCollection<Taxlot>();

            SelectedTradeType = TradeTypeStrings[0];
            SelectedTermType = TradeTermStrings[0];
            SelectedDurationType = TradeDurationStrings[0];            
            ChartSubtitle = "All Positions";
            PreviewButtonText = "Preview Order";
            ShowLimitButtonText = "Show Limit Orders";
            LimitOrdersHidden = true;
            LimitBoxActive = false;
            OrderTermsOK = false;
            LimitPrice = 0;
            TotalValue = 0;
            TotalCostBasis = 0;
            TotalGainLoss = 0;
            AlertBoxMessage = "";
            LimitOrderIsSelected = false;
            _canLoad = true;
            _canSave = true;

            GetLimitOrders();

            //_portfolioManagementService.StartUpdates(); //TURNED OFF FOR TESTING
        }

        private void SetLocalMode(LocalModeMessage message)
        {
            _localMode = message.LocalMode;
        }

        private void CreateTaxlots(TaxlotMessage message)
        {
            Taxlots = new ObservableCollection<Taxlot>(message.Taxlots);
        }

        private void ProcessTradeComplete(TradeCompleteMessage message)
        {
            Taxlots = new ObservableCollection<Taxlot>(message.Taxlots);
            Positions = new ObservableCollection<Position>(message.Positions);
        }

        private void ProcessLimitOrderCreated(LimitOrderMessage message)
        {
            LimitOrderList = new ObservableCollection<LimitOrder>(message.LimitOrders);
        }

        private void ProcessStockDataResponse(StockDataResponseMessage message)
        {
            if (message.IsStartup)
                return;

            if (message.Securities != null)
            {
                _requestedSecurityList = message.Securities;
            }

            if (message.Security != null)
            {
                _requestedSecurity = message.Security;
            }
        }

        private void CreatePositions(PositionMessage message)
        {
            if (message.IsStartup)
            {
                Positions = new ObservableCollection<Position>(message.Positions);
            }

        }

        private void GetValueTotals()
        {
            var totalValue = 0M;
            var totalGainLoss = 0M;
            var totalCostBasis = 0M;

            foreach (var pos in Positions)
            {
                if (_showAllPositions)
                {
                    totalValue += pos.MarketValue;
                    totalGainLoss += pos.GainLoss;
                    totalCostBasis += pos.CostBasis;
                }
                else if (_showStocksOnly && pos.Security is Stock)
                {
                    totalValue += pos.MarketValue;
                    totalGainLoss += pos.GainLoss;
                    totalCostBasis += pos.CostBasis;
                }
                else if (_showFundsOnly && pos.Security is MutualFund)
                {
                    totalValue += pos.MarketValue;
                    totalGainLoss += pos.GainLoss;
                    totalCostBasis += pos.CostBasis;
                }
            }

            TotalValue = totalValue;
            TotalCostBasis = totalCostBasis;
            TotalGainLoss = totalGainLoss;
        }

        private void GetPositions() //Don't put async here...
        {
            var positions = _portfolioManagementService.GetPositions();

            if(positions != null)            
                Positions = new ObservableCollection<Position>(positions);
            
        }

        private void GetTaxlots()
        {
            var lots = _portfolioManagementService.GetTaxlots();

            if (lots != null)
            {
                Taxlots = new ObservableCollection<Taxlot>(lots);
            }
        }

        private void ExecuteShowAllSecurities()
        {
            _showStocksOnly = false;
            _showFundsOnly = false;
            _showAllPositions = true;

            ChartSubtitle = "All Positions";
            AllocationChartPositions = _portfolioManagementService.GetChartAllSecurities();

            ClearHiddenList();            

            GetValueTotals();
        }

        private void ExecuteShowStocksOnly()
        {
            _showStocksOnly = true;
            _showFundsOnly = false;
            _showAllPositions = false;
            ChartSubtitle = "Stocks only";
            AllocationChartPositions = _portfolioManagementService.GetChartStocksOnly();

            ClearHiddenList();

            _hiddenPositions = new List<Position>();
            var trimmedList = new List<Position>(Positions);

            foreach (var pos in Positions)
            {
                if (pos.Security is MutualFund) { 
                    trimmedList.Remove(pos);
                    _hiddenPositions.Add(pos);
                }
            }

            Positions = new ObservableCollection<Position>(trimmedList.OrderBy(t => t.Ticker));

            GetValueTotals();
        }

        private void ExecuteShowFundsOnly()
        {
            _showStocksOnly = false;
            _showFundsOnly = true;
            _showAllPositions = false;

            ChartSubtitle = "Mutual Funds only";
            AllocationChartPositions = _portfolioManagementService.GetChartFundsOnly();

            ClearHiddenList();

            _hiddenPositions = new List<Position>();
            var trimmedList = new List<Position>(Positions);

            foreach (var pos in Positions)
            {
                if (pos.Security is Stock)
                {
                    trimmedList.Remove(pos);
                    _hiddenPositions.Add(pos);
                }
            }

            Positions = new ObservableCollection<Position>(trimmedList.OrderBy(t => t.Ticker));


            GetValueTotals();
        }

        private void ClearHiddenList()
        {
            var listToSort = new List<Position>(Positions);

            if (_hiddenPositions != null && _hiddenPositions.Count > 0)
            {
                foreach (var pos in _hiddenPositions)
                {
                    listToSort.Add(pos);
                }
                _hiddenPositions.Clear();
            }

            Positions = new ObservableCollection<Position>(listToSort.OrderBy(t => t.Ticker));
        }

        private void RefreshCollection(DatabaseMessage message)
        {
            if (message.PositionsSuccessful)
            {
                GetPositions();
                GetTaxlots();
                GetValueTotals();
            }

            if (AllocationChartPositions == null && Positions != null)
            {
                ExecuteShowAllSecurities();
            }
        }

        private async Task ExecutePreviewOrder()
        {
            _previewOrderIsBusy = true;

            var orderOk = await CheckOrderTerms();
            if (orderOk)
            {
                //Send stock preview request
                Messenger.Default.Send<StockDataRequestMessage>(new StockDataRequestMessage(_orderTickerText, false,
                    true));                
            }
            else
            {
                SetAlertMessage(new TradeMessage(_orderTickerText, _orderShareQuantity));
                AlertBoxVisible = true;
                OrderTermsOK = false;
            }
            _previewOrderIsBusy = false;
        }

        private void HandleStockDataResponse(StockDataResponseMessage message)
        {
            if (message.IsPreview)
            {
                PreviewSecurity = message.Security;

                if (PreviewSecurity is Stock)
                {
                    PreviewPrice = PreviewSecurity.LastPrice;
                    PreviewDescription = PreviewSecurity.Description;
                    PreviewVolume = ((Stock)PreviewSecurity).Volume.ToString();
                    PreviewAsk = ((Stock)PreviewSecurity).Ask.ToString();
                    PreviewAskSize = ((Stock)PreviewSecurity).AskSize.ToString();
                    PreviewBid = ((Stock)PreviewSecurity).Bid.ToString();
                    PreviewBidSize = ((Stock)PreviewSecurity).BidSize.ToString();
                }
                else if (PreviewSecurity is MutualFund)
                {
                    PreviewPrice = PreviewSecurity.LastPrice;
                    PreviewDescription = PreviewSecurity.Description;
                    PreviewVolume = "Mutual Fund: No Volume";
                    PreviewAsk = "-";
                    PreviewAskSize = "-";
                    PreviewBid = "-";
                    PreviewBidSize = "-";
                }

                AlertBoxVisible = false;
                ExecuteButtonEnabled = true;
            }
        }

        private async Task<bool> CheckOrderTerms()
        {
            //Check ticker, share, and secType to see if they are valid
            var tickerNotEmpty = !string.IsNullOrEmpty(_orderTickerText);
            var shareQuantityValid = (_orderShareQuantity > 0);
            var securityTypeValid = (_selectedSecurityType is Stock || _selectedSecurityType is MutualFund);

            //Check to see that selected security type matches the ticker
            bool secTypeMatch;
            var tickerSecType = await _portfolioManagementService.GetSecurityType(_orderTickerText, _selectedTradeType);

            if (tickerSecType == null)
            {
                var errorMessage = @"Your selected security type does not match the ticker's security type.";
                Messenger.Default.Send(new TradeMessage(_orderTickerText,_orderShareQuantity, errorMessage));
                return false;
            }
                

            if (_selectedSecurityType is Stock && tickerSecType is Stock)
                secTypeMatch = true;
            else if (_selectedSecurityType is MutualFund && tickerSecType is MutualFund)
                secTypeMatch = true;
            else
                secTypeMatch = false;

            if (tickerNotEmpty && shareQuantityValid && securityTypeValid && secTypeMatch)
            {
                OrderTermsOK = true;
                PreviewButtonText = "Order Terms OK";
                return true;
            }                
            else
            {
                OrderTermsOK = false;
                PreviewButtonText = "Preview Order";
                return false;
            }            
        }

        private async void ExecuteScreenerPreview(string screenerTicker)
        {
            if (!string.IsNullOrEmpty(screenerTicker))
            {
                var resultSecurity = await _portfolioManagementService.GetTradePreviewSecurity(screenerTicker);
                ScreenerStock = resultSecurity;
            }
        }

        private void ExecuteExecuteOrder()
        {
            var newTrade = new Trade(SelectedTradeType, _previewSecurity, _orderTickerText, _orderShareQuantity, _selectedTermType, _limitPrice, _selectedDurationType);
            if (SelectedTradeType == "Buy")
                _portfolioManagementService.Buy(newTrade);
            else if (SelectedTradeType == "Sell")
                _portfolioManagementService.Sell(newTrade);
            GetPositions();
            GetTaxlots();
            GetLimitOrders();
            ExecuteShowAllSecurities();

            OrderTickerText = "";
            OrderShareQuantity = 0;
            SelectedTradeType = TradeTypeStrings[0];
            SelectedTermType = TradeTermStrings[0];
            SelectedDurationType = TradeDurationStrings[0];
            LimitPrice = 0;
            PreviewPrice = 0;
            PreviewBid = "";
            PreviewAsk = "";
            PreviewAskSize = "";
            PreviewBidSize = "";
            PreviewDescription = "";
            PreviewVolume = "";
            SelectedSecurityType = SecurityTypes[0];

            OrderTermsOK = false;           
            ExecuteButtonEnabled = false;
        }

        private void GetLimitOrders()
        {
            var limitOrders = _portfolioManagementService.GetLimitOrders();
            LimitOrderList = new ObservableCollection<LimitOrder>(limitOrders);
        }

        private void PopulateSelectedSecurityTerms()
        {
            OrderTickerText = _selectedPosition.Ticker;
            if (_selectedPosition.Security is Stock)
            {
                SelectedSecurityType = SecurityTypes[0];
            }
            else
            {
                SelectedSecurityType = SecurityTypes[1];
            }
        }

        private void RevertExecuteButtonAndAlert()
        {
            ExecuteButtonEnabled = false;
            PreviewButtonText = "Preview Order";
            AlertBoxMessage = "";
        }    

        private void ExecuteSetIntervalTenSeconds()
        {
            var span = new TimeSpan(0, 0, 10);
            _portfolioManagementService.UpdateTimerInterval(span);
        }

        private void ExecuteSetIntervalThirtySeconds()
        {
            var span = new TimeSpan(0, 0, 30);
            _portfolioManagementService.UpdateTimerInterval(span);
        }

        private void ExecuteSetIntervalSixtySeconds()
        {
            var span = new TimeSpan(0, 0, 60);
            _portfolioManagementService.UpdateTimerInterval(span);
        }

        private void ExecuteSetIntervalFiveMinutes()
        {
            var span = new TimeSpan(0, 5, 0);
            _portfolioManagementService.UpdateTimerInterval(span);
        }

        private void ExecuteToggleLimitDatagrid()
        {
            if (LimitOrdersHidden)
            {
                LimitOrdersHidden = false;
                ShowLimitButtonText = "Hide Limit Orders";                
            }
            else
            {
                LimitOrdersHidden = true;
                SelectedLimitOrder = null;
                ShowLimitButtonText = "Show Limit Orders";
            }
        }

        private void ExecuteDeletePortfolio()
        {
            _portfolioManagementService.DeletePortfolio();
            GetPositions();
            GetTaxlots();
            ExecuteShowAllSecurities();
        }

        private async Task ExecuteSavePortfolio()
        {
            using (var portFileOps = new PortfolioFileOps())
            {
                var saved = await portFileOps.TrySaveSession(new SessionData(LimitOrderList.ToList(), Taxlots.ToList()));
            }
        }

        private async Task ExecuteLoadPortfolio()
        {
            using (var portFileOps = new PortfolioFileOps())
            {
                var sessionData = await portFileOps.TryLoadSession();
                
                //Send locally loaded List<Taxlot> to PortfolioDatabaseService to listen for List<Position>
                //In this case, the message's "IsStartup" boolean is true because loading the Positions
                //Is considered a startup process
                Messenger.Default.Send<TaxlotMessage>(new TaxlotMessage(sessionData.Taxlots, true, true));

                //Create the List<LimitOrder> from saved file
                LimitOrderList = new ObservableCollection<LimitOrder>(sessionData.LimitOrders);

                //Send list of LimitOrders to startup listeners
                Messenger.Default.Send<LimitOrderMessage>(new LimitOrderMessage(sessionData.LimitOrders, true));
            }
        }

        private void ExecuteUpdatePrices()
        {
            _portfolioManagementService.TestLimitOrderMethods();
            GetPositions();
            GetTaxlots();
        }

        private void ExecuteDeleteLimitOrder()
        {
            if (SelectedLimitOrder != null)
            {
                LimitOrderList.Remove(SelectedLimitOrder);
                SelectedLimitOrder = null;
            }                
            else
            {
                var errorMessage = @"Notification: Please select a limit order to delete.";
                Messenger.Default.Send(new TradeMessage("X", 0, errorMessage));                
            }
        }

        private async Task ExecuteCloseApplication()
        {
            if (_localMode)
                await ExecuteSavePortfolio();
            else
                Messenger.Default.Send<ShutdownMessage>(new ShutdownMessage(true));
            System.Windows.Application.Current.Shutdown();
        }

        private bool CanPreview()
        {
            return !_previewOrderIsBusy;
        }

        private bool CanLoad()
        {
            return _canLoad;
        }

        private bool CanSave()
        {
            return _canSave;
        }

        private void SetAlertMessage(TradeMessage message)
        {
            AlertBoxMessage = message.Message;
            AlertBoxVisible = true;
        }

        private void SetAlertMessage(FileErrorMessage message)
        {
            AlertBoxMessage = message.Message;
            AlertBoxVisible = true;
        }    
    }
}