/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Asset_Management_Platform"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using Asset_Management_Platform.Utility;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Asset_Management_Platform.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
                //SimpleIoc.Default.Register<IDataService, DesignDataService>();
                SimpleIoc.Default.Register<IStockDataService, StockDataService>();
                SimpleIoc.Default.Register<IPortfolioManagementService, PortfolioManagementService>();
                SimpleIoc.Default.Register<IPortfolioDatabaseService, PortfolioDatabaseService>();
                SimpleIoc.Default.Register<IChartService, ChartService>();
                SimpleIoc.Default.Register<YahooAPIService>();
                SimpleIoc.Default.Register<SecurityTableSeederDataService>();
                SimpleIoc.Default.Register<MainViewModel>();
            }
            else
            {
                
                SimpleIoc.Default.Register<IStockDataService, StockDataService>();
                SimpleIoc.Default.Register<IPortfolioManagementService, PortfolioManagementService>();
                SimpleIoc.Default.Register<IPortfolioDatabaseService, PortfolioDatabaseService>();
                SimpleIoc.Default.Register<YahooAPIService>();
                SimpleIoc.Default.Register<SecurityTableSeederDataService>();
                SimpleIoc.Default.Register<MainViewModel>();
            }
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }
        
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}