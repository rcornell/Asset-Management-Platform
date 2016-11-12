using Asset_Management_Platform.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System;
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


        public ObservableCollection<String> TradeTypeStrings;
        public string SelectedTradeType;

        private string _orderTickerText;
        public string OrderTickerText
        {
            get { return _orderTickerText; }
            set { _orderTickerText = value;
                RaisePropertyChanged(() => OrderTickerText); }
        }


        private int _orderShareQuantity;
        public int OrderShareQuantity
        {
            get { return _orderShareQuantity; }
            set { _orderShareQuantity = value;
                RaisePropertyChanged(() => OrderShareQuantity);
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


        public DisplayStock SelectedDisplayStock;

        public RelayCommand AddPosition
        {
            get { return new RelayCommand(ExecuteAddPosition); }
        }

        public RelayCommand SellPosition
        {
            get { return new RelayCommand(ExecuteSellPosition); }
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
            TradeTypeStrings = new ObservableCollection<string>() { "Buy", "Sell" };

            //if (IsInDesignModeStatic)
            //{
            //    StockList = new ObservableCollection<DisplayStock>()
            //    {
            //        new DisplayStock(new Position("AAPL", 100), new Stock("", "AAPL", "Apple Computers, Inc.", float.Parse("110.50"), 1.01))
            //    };
            //}

            //SOLVE THE 8 SECOND PAUSE WHEN POLLING YAHOO.



            _portfolioService = portfolioService;
            Messenger.Default.Register<PortfolioMessage>(this, RefreshCollection);
            //_portfolioService.StartUpdates(); //TURNED OFF FOR TESTING

            GetDisplayStocks();

        }

        private void GetDisplayStocks()
        {
            var displayStocks = _portfolioService.GetDisplayStocks();
            StockList = new ObservableCollection<DisplayStock>(displayStocks);
        }

        private void RefreshCollection(PortfolioMessage obj)
        {
            //_portfolio = _portfolioService.GetPortfolio();
        }

        private void ExecuteAddPosition()
        {
            _portfolioService.AddPosition(_orderTickerText, _orderShareQuantity);
            GetDisplayStocks();
        }

        private void ExecuteSellPosition()
        {
            
            _portfolioService.SellPosition(_orderTickerText, _orderShareQuantity);
            GetDisplayStocks();
        }

        private void ExecutePreviewOrder()
        {
            Security previewStock;
            if (!string.IsNullOrEmpty(_orderTickerText) && _orderShareQuantity > 0)
            {
                previewStock = _portfolioService.GetOrderPreviewStock(_orderTickerText);
                PreviewPrice = previewStock.LastPrice;
            }
        }

        private void ExecuteExecuteOrder()
        {
            if (SelectedTradeType == "Buy")
                _portfolioService.AddPosition(_orderTickerText, _orderShareQuantity);
            else
                _portfolioService.SellPosition(_orderTickerText, _orderShareQuantity);
        }

        private void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}