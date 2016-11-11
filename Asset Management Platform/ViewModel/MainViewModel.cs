using Asset_Management_Platform.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
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

        private IPortfolioManagementService _portfolioService;
        public ObservableCollection<Security> SecurityList;

        public MainViewModel(IPortfolioManagementService portfolioService)
        {
 
            //SOLVE THE 8 SECOND PAUSE WHEN POLLING YAHOO.



            _portfolioService = portfolioService;
            Messenger.Default.Register<PortfolioMessage>(this, RefreshCollection);
            //_portfolioService.StartUpdates(); //TURNED OFF FOR TESTING

            GetSecurityList();

        }

        private void GetSecurityList()
        {
            
        }

        private void RefreshCollection(PortfolioMessage obj)
        {
            //_portfolio = _portfolioService.GetPortfolio();
        }

        private void Initialize()
        {
            throw new NotImplementedException();
        }
    }
}