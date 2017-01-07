using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Utility
{
    public class MarkitAPIService
    {

        //private List<Security> _securitiesWithMarketData;

        ////public List<string> Tickers
        ////{
        ////    get { return _tickers; }
        ////    set { _tickers = value; }
        ////}

        //public MarkitAPIService()
        //{

        //}       

        //public Security GetSingleSecurity(string ticker)
        //{

        //    return new Security("","","",0,0.00);
        //}

        //public List<Security> GetMultipleStocks(List<Security> securities)
        //{



              
                
        //            _securitiesWithMarketData = new List<Security>(); //Instantiate the list to return

       
        //            //Should find a way to determine at runtime whether each item is a stock, mutual fund, or ETF.
        //            //Compare tickers to list of tickers that determine which ticker is which type of security
        //            //Use LINQ?

        //                string description = "";
        //                string cusip = "";
        //                decimal lastPrice;
        //                double yield = 0;
        //                double bid = 0;
        //                double ask = 0;
        //                string marketCap = "0";
        //                double peRatio = 0;
        //                int volume = 0;
        //                int bidSize = 0;
        //                int askSize = 0;

        //                bool descriptionIsNA = false;
        //                bool lastPriceIsNA = false;
        //                bool yieldIsNA = false;
        //                bool bidIsNA = false;
        //                bool askIsNA = false;
        //                bool marketCapIsNA = false;
        //                bool peRatioIsNA = false;
        //                bool volumeIsNA = false;
        //                bool bidSizeIsNA = false;
        //                bool askSizeIsNA = false;
                        

        //                //_securitiesWithMarketData.Add(new Stock(cusip, securities[j].Ticker, description, lastPrice, yield, bid, ask, floatMarketCap, peRatio, volume, bidSize, askSize));
        //            return _securitiesWithMarketData;
                

            
        //}

        //private bool IsSecurityUnavailable(bool lastPriceIsNA, bool yieldIsNA, bool marketCapIsNA, bool bidIsNA, bool askIsNA, bool peRatioIsNA, bool volumeIsNA, bool bidSizeIsNA, bool askSizeIsNA, bool descriptionIsNA)
        //{
        //    int numberOfNA = 0;

        //    if (lastPriceIsNA) numberOfNA++;
        //    if (yieldIsNA) numberOfNA++;
        //    if (marketCapIsNA) numberOfNA++;
        //    //if (bidIsNA) numberOfNA++; these can be N/A after market hours
        //    //if (askIsNA) numberOfNA++;
        //    if (peRatioIsNA) numberOfNA++;
        //    if (volumeIsNA) numberOfNA++;
        //    //if (bidSizeIsNA) numberOfNA++; these can be N/A after market hours
        //    //if (askSizeIsNA) numberOfNA++;

        //    if (numberOfNA > 2)
        //        return true;

        //    return false;
        //}

        //public void Dispose()
        //{
        //    _securitiesWithMarketData = null;
        //}
    }
}
