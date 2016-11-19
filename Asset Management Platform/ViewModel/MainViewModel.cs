using Asset_Management_Platform.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Windows.Data;
using Asset_Management_Platform.Messages;

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



        //Customers = new ListCollectionView(_customers);
        //Customers.GroupDescriptions.Add(new PropertyGroupDescription("Gender"));


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
                set { _limitPrice = value ;
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
                AlertBoxMessage = "";
            }
        }


        private int _orderShareQuantity;
        public int OrderShareQuantity
        {
            get { return _orderShareQuantity; }
            set { _orderShareQuantity = value;
                RaisePropertyChanged(() => OrderShareQuantity);
                _executeButtonEnabled = false;
                AlertBoxMessage = "";
            }
        }

        private float _previewPrice;
        public float PreviewPrice
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

        public float PreviewValue
        {
            get { return (_previewPrice * _orderShareQuantity); }
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

        public RelayCommand PreviewOrder
        {
            get { return new RelayCommand(ExecutePreviewOrder); }
        }

        public RelayCommand ExecuteOrder
        {
            get { return new RelayCommand(ExecuteExecuteOrder); }
        }

        private IPortfolioManagementService _portfolioService;

        private ObservableCollection<DisplayStock> _stockList;
        public ObservableCollection<DisplayStock> StockList
        {
            get { return _stockList;}
            set { _stockList = value;
                RaisePropertyChanged(() => StockList);
            }
        }

        public MainViewModel(IPortfolioManagementService portfolioService)
        {
            

            SelectedDisplayStock = null;
            TradeTypeStrings = new ObservableCollection<string>() { " ", "Buy", "Sell" };
            TradeTermStrings = new ObservableCollection<string>() { "Market", "Limit", "Stop", "Stop Limit" };
            TradeDurationStrings = new ObservableCollection<string> { "Day", "GTC", "Market Close", "Market Open", "Overnight" };
            SelectedDurationType = "Day";
            LimitBoxActive = false;
            LimitPrice = 0;


            //SOLVE THE 8 SECOND PAUSE WHEN POLLING YAHOO.



            _portfolioService = portfolioService;
            Messenger.Default.Register<PortfolioMessage>(this, RefreshCollection);
            Messenger.Default.Register<TradeMessage>(this, SetAlertMessage);
            //_portfolioService.StartUpdates(); //TURNED OFF FOR TESTING

            GetDisplayStocks();
            DisplayStockCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("Ticker"));
        }

        private void GetDisplayStocks()
        {
            var displayStocks = _portfolioService.GetDisplayStocks();
            StockList = new ObservableCollection<DisplayStock>(displayStocks);
            DisplayStockCollectionView = new ListCollectionView(StockList);
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
                var resultStock = (Stock)_portfolioService.GetOrderPreviewStock(screenerTicker);
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

            SelectedDurationType = "Day";
            OrderTickerText = "";
            OrderShareQuantity = 0;
            SelectedTermType = "";
            SelectedTradeType = " ";
            LimitPrice = 0.00;
            ExecuteButtonEnabled = false;
        }

        private void SetAlertMessage(TradeMessage message)
        {
            AlertBoxMessage = message.Message;
        }
    }
}