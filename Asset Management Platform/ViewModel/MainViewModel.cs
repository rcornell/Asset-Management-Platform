using Asset_Management_Platform.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

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

        public Portfolio Portfolio;
        public PortfolioService PortfolioService;
        public StockDataService StockDataService;


        public MainViewModel()
        {
            StockDataService = SimpleIoc.Default.GetInstance<StockDataService>("SQLStorageConnection");
            StockDataService.Initialize();
            var securityList = StockDataService.LoadDatabase();
            PortfolioService = SimpleIoc.Default.GetInstance<PortfolioService>();
            //^^^^ This needs a param passed to it. Incorrect use of SimpleIOC?*****

            
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }


        

        
    }
}