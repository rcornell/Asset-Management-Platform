using Asset_Management_Platform.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Asset_Management_Platform.Messages;
using System.ComponentModel;

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


        private Stock _screenerStock;
        public Stock ScreenerStock
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


        private double _limitPrice;
        public double LimitPrice {
            get { return _limitPrice; }
            set { _limitPrice = value;
                _executeButtonEnabled = false;
                RaisePropertyChanged(() => LimitPrice); }
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

        private Stock _previewStock;

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

        private IPortfolioManagementService _portfolioService;

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
            SelectedDurationType = "Day";
            LimitBoxActive = false;
            LimitPrice = 0;
            TotalValue = 0;

            _portfolioService = portfolioService;
            Messenger.Default.Register<PortfolioMessage>(this, RefreshCollection);
            Messenger.Default.Register<TradeMessage>(this, SetAlertMessage);
            //_portfolioService.StartUpdates(); //TURNED OFF FOR TESTING

            GetDisplayStocks();
            GetAllocationChartPositions();
            DisplayStockCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Ticker"));
        }

        private void GetDisplayStocks()
        {
            var displayStocks = _portfolioService.GetDisplayStocks();
            StockList = new ObservableCollection<DisplayStock>(displayStocks);
            DisplayStockCollectionView = new ListCollectionView(StockList);
        }

        private void GetAllocationChartPositions()
        {
            var displayStocks = _portfolioService.GetDisplayStocks();
            decimal totalValue = 0;

            foreach (var stock in displayStocks)
            {
                totalValue += decimal.Parse(stock.MarketValue);
            }

            TotalValue = totalValue;

            ObservableCollection<PositionByWeight> posByWeight = new ObservableCollection<PositionByWeight>();
            foreach (var stock in displayStocks)
            {
                decimal weight = (decimal.Parse(stock.MarketValue) / totalValue) * 100;
                posByWeight.Add(new PositionByWeight(stock.Ticker, System.Math.Round(weight,2)));
            }
            AllocationChartPositions = posByWeight;
        }

        private void RefreshCollection(PortfolioMessage obj)
        {
            //_portfolio = _portfolioService.GetPortfolio();
        }

        private void ExecutePreviewOrder()
        {
            
            if (!string.IsNullOrEmpty(_orderTickerText) && _orderShareQuantity > 0)
            {
                _previewStock = (Stock)_portfolioService.GetOrderPreviewStock(_orderTickerText);
                PreviewPrice = _previewStock.LastPrice;
                PreviewDescription = _previewStock.Description;
                PreviewVolume = _previewStock.Volume.ToString();
                PreviewAsk = _previewStock.Ask.ToString();
                PreviewAskSize = _previewStock.AskSize.ToString();
                PreviewBid = _previewStock.Bid.ToString();
                PreviewBidSize = _previewStock.BidSize.ToString();
                ExecuteButtonEnabled = true;
            }
        }

        private void ExecuteScreenerPreview(string screenerTicker)
        {

            if (!string.IsNullOrEmpty(screenerTicker))
            {
                var resultStock = _portfolioService.GetOrderPreviewStock(screenerTicker);
                ScreenerStock = resultStock;
            }
        }

        private void ExecuteExecuteOrder()
        {
            if (SelectedTradeType == "Buy")
                _portfolioService.AddPosition(_previewStock, _orderTickerText, _orderShareQuantity);
            else if (SelectedTradeType == "Sell")
                _portfolioService.SellPosition(_previewStock, _orderTickerText, _orderShareQuantity);
            GetDisplayStocks();
            GetAllocationChartPositions();

            SelectedDurationType = "Day";
            OrderTickerText = "";
            OrderShareQuantity = 0;
            SelectedTermType = "";
            SelectedTradeType = " ";
            LimitPrice = 0.00;
            PreviewPrice = 0;
            PreviewBid = "";
            PreviewAsk = "";
            PreviewAskSize = "";
            PreviewBidSize = "";
            PreviewDescription = "";
            PreviewVolume = "";
           
            ExecuteButtonEnabled = false;
        }

        public void ExecuteDeletePortfolio()
        {
            _portfolioService.DeletePortfolio();
            GetDisplayStocks();
            GetAllocationChartPositions();
        }

        public void ExecuteSavePortfolio()
        {
            _portfolioService.UploadPortfolio();
        }

        private void ExecuteCloseApplication()
        {
            _portfolioService.UploadPortfolio();
            System.Windows.Application.Current.Shutdown();
        }

        private void SetAlertMessage(TradeMessage message)
        {
            AlertBoxMessage = message.Message;
            AlertBoxVisible = true;
        }
    }
}