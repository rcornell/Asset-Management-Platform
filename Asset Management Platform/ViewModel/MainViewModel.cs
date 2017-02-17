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


        private bool _limitOrdersHidden;

        public bool LimitOrdersHidden
        {
            get { return _limitOrdersHidden; }
            set
            {
                _limitOrdersHidden = value;
                RaisePropertyChanged(() => LimitOrdersHidden);
            }
        }

        private string _previewButtonText;

        public string PreviewButtonText
        {
            get { return _previewButtonText; }
            set
            {
                _previewButtonText = value;
                RaisePropertyChanged(() => PreviewButtonText);
            }
        }

        private bool _orderTermsOK;

        public bool OrderTermsOK
        {
            get { return _orderTermsOK; }
            set
            {
                _orderTermsOK = value;
                RaisePropertyChanged(() => OrderTermsOK);
            }
        }

        private ListCollectionView _limitOrderCollectionView;
        public ListCollectionView LimitOrderCollectionView
        {
            get { return _limitOrderCollectionView; }
            set
            {
                _limitOrderCollectionView = value;
                RaisePropertyChanged(() => LimitOrderCollectionView);
            }
        }

        private ListCollectionView _positionCollectionView;
        public ListCollectionView PositionCollectionView
        {
            get { return _positionCollectionView; }
            set
            {
                _positionCollectionView = value;
                RaisePropertyChanged(() => PositionCollectionView);
            }
        }

        private ListCollectionView _taxlotsCollectionView;
        public ListCollectionView TaxlotsCollectionView
        {
            get { return _taxlotsCollectionView; }
            set
            {
                _taxlotsCollectionView = value;
                RaisePropertyChanged(() => TaxlotsCollectionView);
            }
        }


        private Security _screenerStock;
        public Security ScreenerStock
        {
            get { return _screenerStock; }
            set { _screenerStock = value;
                RaisePropertyChanged(() => ScreenerStock);
            }
        }


        private string _stockScreenerTicker;
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

        private bool _alertBoxVisible;
        public bool AlertBoxVisible {
            get { return _alertBoxVisible; }
            set { _alertBoxVisible = value;
                RaisePropertyChanged(() => AlertBoxVisible); }
        }

        private string _alertBoxMessage;
        public string AlertBoxMessage {
            get { return _alertBoxMessage; }
            set { _alertBoxMessage = value;
                RaisePropertyChanged(() => AlertBoxMessage);
            }
        }

        private bool _executeButtonEnabled;
        public bool ExecuteButtonEnabled
        {
            get { return _executeButtonEnabled; }
            set
            {
                _executeButtonEnabled = value;
                RaisePropertyChanged(() => ExecuteButtonEnabled);
            }
        }

        private bool _limitBoxActive;
        public bool LimitBoxActive
        {
            get { return _limitBoxActive; }
            set { _limitBoxActive = value;
                RaisePropertyChanged(() => LimitBoxActive);
            }
        }

        private bool _limitOrderIsSelected;
        public bool LimitOrderIsSelected
        {
            get { return _limitOrderIsSelected; }
            set
            {
                _limitOrderIsSelected = value;
                RaisePropertyChanged(() => LimitOrderIsSelected);
            }
        }

        private ObservableCollection<string> _tradeDurationStrings;
        public ObservableCollection<string> TradeDurationStrings
        {
            get { return _tradeDurationStrings; }
            set
            {
                _tradeDurationStrings = value;
                RaisePropertyChanged(() => TradeDurationStrings);
            }
        }

        private ObservableCollection<string> _tradeTermStrings;
        public ObservableCollection<string> TradeTermStrings
        {
            get { return _tradeTermStrings; }
            set
            {
                _tradeTermStrings = value;
                RaisePropertyChanged(() => TradeTermStrings);
            }
        }

        private ObservableCollection<string> _tradeTypeStrings;
        public ObservableCollection<string> TradeTypeStrings
        {
            get { return _tradeTypeStrings; }
            set
            {
                _tradeTypeStrings = value;
                RaisePropertyChanged(() => TradeTypeStrings);
            }
        }

        private ObservableCollection<Security> _securityTypeStrings;
        public ObservableCollection<Security> SecurityTypes
        {
            get { return _securityTypeStrings; }
            set
            {
                _securityTypeStrings = value;
                RaisePropertyChanged(() => SecurityTypes);
            }
        }

        private ObservableCollection<LimitOrder> _limitOrderList;
        public ObservableCollection<LimitOrder> LimitOrderList
        {
            get { return _limitOrderList; }
            set
            {
                _limitOrderList = value;
                RaisePropertyChanged(() => LimitOrderList);
            }
        }

        private LimitOrder _selectedLimitOrder;

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


        private string _chartSubtitle;
        public string ChartSubtitle {
            get { return _chartSubtitle; }
            set { _chartSubtitle = value;
                RaisePropertyChanged(() => ChartSubtitle);
            }
        }

        private string _previewDescription;
        public string PreviewDescription {
            get { return _previewDescription; }
            set { _previewDescription = value;
                RaisePropertyChanged(() => PreviewDescription);
            }
        }

        private string _previewVolume;
        public string PreviewVolume
        {
            get { return _previewVolume; }
            set
            {
                _previewVolume = value;
                RaisePropertyChanged(() => PreviewVolume);
            }
        }

        private string _previewAsk;
        public string PreviewAsk
        {
            get { return _previewAsk; }
            set
            {
                _previewAsk = value;
                RaisePropertyChanged(() => PreviewAsk);
            }
        }

        private string _previewAskSize;
        public string PreviewAskSize
        {
            get { return _previewAskSize; }
            set
            {
                _previewAskSize = value;
                RaisePropertyChanged(() => PreviewAskSize);
            }
        }

        private string _previewBid;
        public string PreviewBid
        {
            get { return _previewBid; }
            set
            {
                _previewBid = value;
                RaisePropertyChanged(() => PreviewBid);
            }
        }

        private string _previewBidSize;
        public string PreviewBidSize
        {
            get { return _previewBidSize; }
            set
            {
                _previewBidSize = value;
                RaisePropertyChanged(() => PreviewBidSize);
            }
        }


        private decimal _limitPrice;
        public decimal LimitPrice {
            get { return _limitPrice; }
            set { _limitPrice = value;
                _executeButtonEnabled = false;
                RaisePropertyChanged(() => LimitPrice);
                RevertExecuteButtonAndAlert();
            }
        }


        private Security _selectedSecurityType;
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

        private string _selectedDurationType;
        public string SelectedDurationType {
            get { return _selectedDurationType; }
            set
            {
                _selectedDurationType = value;
                RaisePropertyChanged(() => SelectedDurationType);
                RevertExecuteButtonAndAlert();
            }
        }


        private string _selectedTermType;
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

        private string _selectedTradeType;
        public string SelectedTradeType
        {
            get { return _selectedTradeType; }
            set { _selectedTradeType = value;
                RaisePropertyChanged(() => SelectedTradeType);
                RevertExecuteButtonAndAlert();
            }
        }

        private string _orderTickerText;
        public string OrderTickerText
        {
            get { return _orderTickerText; }
            set { _orderTickerText = value.ToUpper();
                RaisePropertyChanged(() => OrderTickerText);
                RevertExecuteButtonAndAlert();
            }
        }


        private int _orderShareQuantity;
        public int OrderShareQuantity
        {
            get { return _orderShareQuantity; }
            set { _orderShareQuantity = value;
                RaisePropertyChanged(() => OrderShareQuantity);
                RevertExecuteButtonAndAlert();
            }
        }

        private decimal _previewPrice;
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

        private Position _selectedPosition;
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

        #region All Commands

        public RelayCommand DeletePortfolio
        {
            get { return new RelayCommand(ExecuteDeletePortfolio); }
        }
        public RelayCommand SavePortfolio
        {
            get { return new RelayCommand(ExecuteSavePortfolio); }
        }         

        public RelayCommand CloseApplication
        {
            get { return new RelayCommand(ExecuteCloseApplication); }
        }

        public RelayCommand PreviewOrder
        {
            get { return new RelayCommand(ExecutePreviewOrder); }
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

        public decimal _totalValue;
        public decimal TotalValue {
            get { return _totalValue; }
            set { _totalValue = value;
                RaisePropertyChanged(() => TotalValue); }
        }

        public decimal _totalCostBasis;
        public decimal TotalCostBasis
        {
            get { return _totalCostBasis; }
            set
            {
                _totalCostBasis = value;
                RaisePropertyChanged(() => TotalCostBasis);
            }
        }

        public decimal _totalGainLoss;
        public decimal TotalGainLoss
        {
            get { return _totalGainLoss; }
            set
            {
                _totalGainLoss = value;
                RaisePropertyChanged(() => TotalGainLoss);
            }
        }

        private ObservableCollection<PositionByWeight> _allocationChartPositions;
        public ObservableCollection<PositionByWeight> AllocationChartPositions {
            get {
                return _allocationChartPositions;
            }
            set {
                _allocationChartPositions = value;
                RaisePropertyChanged(() => AllocationChartPositions);
            }
        }

        private IPortfolioManagementService _portfolioManagementService;

        private Security _previewSecurity;
        public Security PreviewSecurity {
            get { return _previewSecurity; }
            set { _previewSecurity = value;
                RaisePropertyChanged(() => PreviewSecurity);
            }

        }

        private ObservableCollection<Taxlot> _taxlots;
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

        private ObservableCollection<Position> _positions;
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


        public MainViewModel(IPortfolioManagementService portfolioService)
        {
            TradeTypeStrings = new ObservableCollection<string>() { " ", "Buy", "Sell" };
            TradeTermStrings = new ObservableCollection<string>() { "Market", "Limit", "Stop", "Stop Limit" };
            TradeDurationStrings = new ObservableCollection<string> { "Day", "GTC", "Market Close", "Market Open", "Overnight" };
            SecurityTypes = new ObservableCollection<Security> { new Stock(), new MutualFund() };
            SelectedDurationType = "Day";
            ChartSubtitle = "All Positions";
            PreviewButtonText = "Preview Order";
            LimitBoxActive = false;
            OrderTermsOK = false;
            LimitPrice = 0;
            TotalValue = 0;
            AlertBoxMessage = "";
            LimitOrderIsSelected = false;

            _portfolioManagementService = portfolioService;
            Messenger.Default.Register<PortfolioMessage>(this, RefreshCollection);
            Messenger.Default.Register<TradeMessage>(this, SetAlertMessage);
            //_portfolioManagementService.StartUpdates(); //TURNED OFF FOR TESTING

            GetPositions();
            GetTaxlots();
            GetLimitOrders();
            ExecuteShowAllSecurities();
            PositionCollectionView = new ListCollectionView(Positions);
            PositionCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Ticker"));
            TaxlotsCollectionView = new ListCollectionView(Taxlots);
            TaxlotsCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Ticker"));



        }


        private void GetPositions()
        {
            var positions = _portfolioManagementService.GetPositions();
            Positions = new ObservableCollection<Position>(positions);
        }

        private void GetTaxlots()
        {
            var lots = _portfolioManagementService.GetTaxlots();
            Taxlots = new ObservableCollection<Taxlot>(lots);
        }

        private void ExecuteShowAllSecurities()
        {
            ChartSubtitle = "All Positions";
            AllocationChartPositions = _portfolioManagementService.GetChartAllSecurities();
            foreach (var sec in Positions)
            {
                sec.Hidden = false;
            }
        }

        private void ExecuteShowStocksOnly()
        {
            ChartSubtitle = "Stocks only";
            AllocationChartPositions = _portfolioManagementService.GetChartStocksOnly();

            foreach (var sec in Positions)
            {
                if (sec.Security is MutualFund)
                    sec.Hidden = true;
                else
                    sec.Hidden = false;
            }   
        }

        private void ExecuteShowFundsOnly()
        {
            ChartSubtitle = "Mutual Funds only";
            AllocationChartPositions = _portfolioManagementService.GetChartFundsOnly();
            foreach (var sec in Positions)
            {
                if (sec.Security is Stock)
                    sec.Hidden = true;
                else
                    sec.Hidden = false;
            }
        }

        private void ExecuteUpdatePrices()
        {
            _portfolioManagementService.TestLimitOrderMethods();
            GetPositions();
            GetTaxlots();
        }

        private void RefreshCollection(PortfolioMessage obj)
        {
            //_portfolio = _portfolioManagementService.GetPortfolio();
        }

        private void ExecutePreviewOrder()
        {
            var orderOk = CheckOrderTerms();     
            if (orderOk)
            {
                PreviewSecurity = _portfolioManagementService.GetOrderPreviewSecurity(_orderTickerText, SelectedSecurityType);

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
            else
            {
                SetAlertMessage(new TradeMessage(_orderTickerText, _orderShareQuantity));
                AlertBoxVisible = true;
                OrderTermsOK = false;
            }
        }

        private bool CheckOrderTerms()
        {
            //Check ticker, share, and secType to see if they are valid
            var tickerNotEmpty = !string.IsNullOrEmpty(_orderTickerText);
            var shareQuantityValid = (_orderShareQuantity > 0);
            var securityTypeValid = (_selectedSecurityType is Stock || _selectedSecurityType is MutualFund);

            //Check to see that selected security type matches the ticker
            bool secTypeMatch;
            var tickerSecType = _portfolioManagementService.GetSecurityType(_orderTickerText, _selectedTradeType);

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

        private void ExecuteScreenerPreview(string screenerTicker)
        {
            if (!string.IsNullOrEmpty(screenerTicker))
            {
                var resultSecurity = _portfolioManagementService.GetOrderPreviewSecurity(screenerTicker);
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

            SelectedDurationType = " ";
            OrderTickerText = "";
            OrderShareQuantity = 0;
            SelectedTermType = " ";
            SelectedTradeType = " ";
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
            LimitOrderCollectionView = new ListCollectionView(limitOrders);
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

        public void ExecuteDeletePortfolio()
        {
            _portfolioManagementService.DeletePortfolio();
            GetPositions();
            GetTaxlots();
            ExecuteShowAllSecurities();
        }

        public void ExecuteSavePortfolio()
        {
            _portfolioManagementService.UploadPortfolio();
        }

        public void ExecuteDeleteLimitOrder()
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

        private void ExecuteCloseApplication()
        {
            _portfolioManagementService.UploadAllDatabases();
            System.Windows.Application.Current.Shutdown();
        }

        private void SetAlertMessage(TradeMessage message)
        {
            AlertBoxMessage = message.Message;
            AlertBoxVisible = true;
        }

        
    }
}