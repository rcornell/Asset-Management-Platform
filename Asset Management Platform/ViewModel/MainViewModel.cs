using Asset_Management_Platform.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System;
using Asset_Management_Platform.Messages;

namespace Asset_Management_Platform.ViewModel
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

        private Portfolio _portfolio;
        private PortfolioService _portfolioService;
        private StockDataService _stockDataService;
        public ObservableCollection<Security> SecurityList;

        public MainViewModel()
        {
            _stockDataService = SimpleIoc.Default.GetInstance<StockDataService>("SQLStorageConnection");
            _stockDataService.Initialize();
            var securityList = _stockDataService.LoadDatabase();
            _portfolioService = SimpleIoc.Default.GetInstance<PortfolioService>();
            //^^^^ This needs a param passed to it. Incorrect use of SimpleIOC?*****

            Messenger.Default.Register<PortfolioMessage>(this, RefreshCollection);

            
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }

        private void RefreshCollection(PortfolioMessage obj)
        {
            _portfolio = _portfolioService.MyPortfolio;
        }

        private void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}