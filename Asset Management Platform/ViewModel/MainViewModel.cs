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

        private ListCollectionView _displayStockCollectionView;
        public ListCollectionView DisplayStockCollectionView
        {
            get { return _displayStockCollectionView; }
            set { _displayStockCollectionView = value;
                RaisePropertyChanged(() => DisplayStockCollectionView);
            }
        }

        private ListCollectionView _displayFundCollectionView;
        public ListCollectionView DisplayFundCollectionView
        {
            get { return _displayFundCollectionView; }
            set
            {
                _displayFundCollectionView = value;
                RaisePropertyChanged(() => DisplayFundCollectionView);
            }
        }

        private ListCollectionView _displaySecurityCollectionView;
        public ListCollectionView DisplaySecurityCollectionView
        {
            get { return _displaySecurityCollectionView; }
            set
            {
                _displaySecurityCollectionView = value;
                RaisePropertyChanged(() => DisplaySecurityCollectionView);
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
                RaisePropertyChanged(() => PreviewDescription); }
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
                RaisePropertyChanged(() => LimitPrice); }
        }


        private Security _selectedSecurityType;
        public Security SelectedSecurityType
        {
            get { return _selectedSecurityType; }
            set
            {
                _selectedSecurityType = value;
                RaisePropertyChanged(() => SelectedSecurityType);
            }
        }

        private string _selectedDurationType;
        public string SelectedDurationType {
            get { return _selectedDurationType; }
            set
            {
                _selectedDurationType = value;
                RaisePropertyChanged(() => SelectedDurationType);
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
                _executeButtonEnabled = false;
            }
        }

        private string _selectedTradeType;
        public string SelectedTradeType
        {
            get { return _selectedTradeType; }
            set { _selectedTradeType = value;
                RaisePropertyChanged(() => SelectedTradeType);
                _executeButtonEnabled = false;
            }
        }

        private string _orderTickerText;
        public string OrderTickerText
        {
            get { return _orderTickerText; }
            set { _orderTickerText = value.ToUpper();
                RaisePropertyChanged(() => OrderTickerText);
                _executeButtonEnabled = false;
            }
        }


        private int _orderShareQuantity;
        public int OrderShareQuantity
        {
            get { return _orderShareQuantity; }
            set { _orderShareQuantity = value;
                RaisePropertyChanged(() => OrderShareQuantity);
                _executeButtonEnabled = false;
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

        

        private DisplayStock _selectedDisplayStock;
        public DisplayStock SelectedDisplayStock
        {
            get { return _selectedDisplayStock; }
            set { _selectedDisplayStock = value;
                RaisePropertyChanged(() => SelectedDisplayStock);
            }
        }

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
        public decimal _totalValue;
        public decimal TotalValue {
            get { return _totalValue; }
            set { _totalValue = value;
                RaisePropertyChanged(() => TotalValue); }
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

        private ObservableCollection<DisplayStock> _stockList;
        public ObservableCollection<DisplayStock> StockList
        {
            get { return _stockList; }
            set { _stockList = value;
                RaisePropertyChanged(() => StockList);
            }
        }

        private ObservableCollection<DisplayMutualFund> _fundList;
        public ObservableCollection<DisplayMutualFund> MutualFundList
        {
            get { return _fundList; }
            set
            {
                _fundList = value;
                RaisePropertyChanged(() => MutualFundList);
            }
        }

        private ObservableCollection<DisplaySecurity> _securityList;
        public ObservableCollection<DisplaySecurity> SecurityList
        {
            get { return _securityList; }
            set
            {
                _securityList = value;
                RaisePropertyChanged(() => SecurityList);
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
        public MainViewModel(IPortfolioManagementService portfolioService)
        {
            //if (IsInDesignMode)
            //{
            //    DisplayStockCollectionView = new ListCollectionView(new ObservableCollection<DisplayStock>()
            //    {
            //        new DisplayStock(new Position("AAPL", 1000), new Stock("", "AAPL", "Apple Inc.", 5, 1.00)),
            //        new DisplayStock(new Position("IBM", 500), new Stock("", "IBM", "Intl Business Machines.", 10, 3.00)),
            //    });

            //    TradeTypeStrings = new ObservableCollection<string>() { " ", "Buy", "Sell" };
            //    TradeTermStrings = new ObservableCollection<string>() { "Market", "Limit", "Stop", "Stop Limit" };
            //    TradeDurationStrings = new ObservableCollection<string> { "Day", "GTC", "Market Close", "Market Open", "Overnight" };
            //}

            SelectedDisplayStock = null;
            TradeTypeStrings = new ObservableCollection<string>() { " ", "Buy", "Sell" };
            TradeTermStrings = new ObservableCollection<string>() { "Market", "Limit", "Stop", "Stop Limit" };
            TradeDurationStrings = new ObservableCollection<string> { "Day", "GTC", "Market Close", "Market Open", "Overnight" };
            SecurityTypes = new ObservableCollection<Security> { new Stock(), new MutualFund() };
            SelectedDurationType = "Day";
            ChartSubtitle = "All Positions";
            LimitBoxActive = false;
            LimitPrice = 0;
            TotalValue = 0;

            _portfolioManagementService = portfolioService;
            Messenger.Default.Register<PortfolioMessage>(this, RefreshCollection);
            Messenger.Default.Register<TradeMessage>(this, SetAlertMessage);
            //_portfolioService.StartUpdates(); //TURNED OFF FOR TESTING

            
            GetDisplaySecurities();
            GetLimitOrders();
            ExecuteShowAllSecurities();
            DisplaySecurityCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Ticker"));
        }

        /// <summary>
        /// Calls the PortfolioManagementService to find the current list
        /// of stocks and mutual funds. Creates observable collections of each
        /// for binding, then creates the list of all securities for binding.
        /// </summary>
        private void GetDisplaySecurities()
        {
            //Get each List<T> for stocks and mutual funds, then 
            //create or update the corresponding ObservableCollection<T>
            var displayStocksList = _portfolioManagementService.GetDisplayStocks();
            StockList = new ObservableCollection<DisplayStock>(displayStocksList);

            var displayMutualFundList = _portfolioManagementService.GetDisplayMutualFunds();
            MutualFundList = new ObservableCollection<DisplayMutualFund>(displayMutualFundList);

            //Instantiate list of all securities, then add all stock and MF items to it.
            SecurityList = new ObservableCollection<DisplaySecurity>();
            
            foreach (var item in StockList)
            {
                SecurityList.Add(item);
            }

            foreach (var item in MutualFundList)
            {
                SecurityList.Add(item);
            }

            //Create or update the ListCollectionViews
            DisplaySecurityCollectionView = new ListCollectionView(SecurityList);
        }

        private void ExecuteShowAllSecurities()
        {
            ChartSubtitle = "All Positions";
            AllocationChartPositions = _portfolioManagementService.GetChartAllSecurities();
        }

        private void ExecuteShowStocksOnly()
        {
            ChartSubtitle = "Stocks only";
            AllocationChartPositions = _portfolioManagementService.GetChartStocksOnly();
        }

        private void ExecuteShowFundsOnly()
        {
            ChartSubtitle = "Mutual Funds only";
            AllocationChartPositions = _portfolioManagementService.GetChartFundsOnly();
        }

        private void ExecuteUpdatePrices()
        {
            _portfolioManagementService.UpdatePortfolioPrices();
            GetDisplaySecurities();
        }

        private void RefreshCollection(PortfolioMessage obj)
        {
            //_portfolio = _portfolioService.GetPortfolio();
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

                ExecuteButtonEnabled = true;
            }
        }

        private bool CheckOrderTerms()
        {
            var tickerNotEmpty = !string.IsNullOrEmpty(_orderTickerText);
            var shareQuantityValid = (_orderShareQuantity > 0);
            var securityTypeValid = (_selectedSecurityType is Stock || _selectedSecurityType is MutualFund);
            if (tickerNotEmpty && shareQuantityValid && securityTypeValid)
                return true;
            return false;
        }

        private void ExecuteScreenerPreview(string screenerTicker)
        {

            if (!string.IsNullOrEmpty(screenerTicker))
            {
                var resultStock = _portfolioManagementService.GetOrderPreviewSecurity(screenerTicker);
                ScreenerStock = resultStock;
            }
        }

        private void ExecuteExecuteOrder()
        {
            var newTrade = new Trade(SelectedTradeType, _previewSecurity, _orderTickerText, _orderShareQuantity, _selectedTermType, _limitPrice, _selectedDurationType);
            if (SelectedTradeType == "Buy")
                _portfolioManagementService.AddPosition(newTrade);
            else if (SelectedTradeType == "Sell")
                _portfolioManagementService.SellPosition(newTrade);
            GetDisplaySecurities();
            GetLimitOrders();
            ExecuteShowAllSecurities();

            SelectedDurationType = "Day";
            OrderTickerText = "";
            OrderShareQuantity = 0;
            SelectedTermType = "Market";
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
           
            ExecuteButtonEnabled = false;
        }

        private void GetLimitOrders()
        {
            var limitOrders = _portfolioManagementService.GetLimitOrders();
            LimitOrderList = new ObservableCollection<LimitOrder>(limitOrders);
        }

        public void ExecuteDeletePortfolio()
        {
            _portfolioManagementService.DeletePortfolio();
            GetDisplaySecurities();
            ExecuteShowAllSecurities();
        }

        public void ExecuteSavePortfolio()
        {
            _portfolioManagementService.UploadPortfolio();
        }

        private void ExecuteCloseApplication()
        {
            _portfolioManagementService.UploadAllToDatabase();
            System.Windows.Application.Current.Shutdown();
        }

        private void SetAlertMessage(TradeMessage message)
        {
            AlertBoxMessage = message.Message;
            AlertBoxVisible = true;
        }
    }
}