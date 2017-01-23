using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Asset_Management_Platform.Utility
{
    class YahooAPIService : IDisposable
    {

        /// <summary>
        /// Yahoo API tags for url string. These
        /// are added the string in sequence with
        /// no separation, e.g. "aa2" for Ask and
        /// Avg Daily Volume.
        //Tag	Meaning
        //a     Ask
        //a2    Average Daily Volume
        //a5    Ask Size
        //b     Bid
        //b2    Ask(Real-time)
        //b3    Bid(Real-time)
        //b4    Book Value
        //b6    Bid Size
        //c     Change & Percent Change
        //c1    Change
        //c3    Commission
        //c6    Change(Real-time)
        //c8    After Hours Change(Real-time)
        //d     Dividend/Share
        //d1    Last Trade Date
        //d2    Trade Date
        //e     Earnings/Share
        //e1    Error Indication(returned for symbol changed / invalid)
        //e7    EPS Estimate Current Year
        //e8    EPS Estimate Next Year
        //e9    EPS Estimate Next Quarter
        //f6    Float Shares
        //g     Day’s Low
        //g1    Holdings Gain Percent
        //g3    Annualized Gain
        //g4    Holdings Gain
        //g5    Holdings Gain Percent(Real-time)
        //g6    Holdings Gain(Real-time)
        //h     Day’s High
        //i     More Info
        //i5    Order Book(Real-time)
        //j	    52-week Low
        //j1    Market Capitalization
        //j3    Market Cap(Real-time)
        //j4    EBITDA
        //j5    Change From 52-week Low
        //j6    Percent Change From 52-week Low
        //k	    52-week High
        //k1    Last Trade(Real-time) With Time
        //k2    Change Percent(Real-time)
        //k3    Last Trade Size
        //k4    Change From 52-week High
        //k5    Percebt Change From 52-week High
        //l     Last Trade(With Time)
        //l1    Last Trade(Price Only)
        //l2    High Limit
        //l3    Low Limit
        //m     Day’s Range
        //m2    Day’s Range(Real-time)
        //m3	50-day Moving Average
        //m4	200-day Moving Average
        //m5    Change From 200-day Moving Average
        //m6    Percent Change From 200-day Moving Average
        //m7    Change From 50-day Moving Average
        //m8    Percent Change From 50-day Moving Average
        //n     Name
        //n4    Notes
        //o     Open
        //p     Previous Close
        //p1    Price Paid
        //p2    Change in Percent
        //p5    Price/Sales
        //p6    Price/Book                         
        //q     Ex-Dividend Date
        //r     P/E Ratio
        //r1    Dividend Pay Date
        //r2    P/E Ratio(Real-time) 
        //r5    PEG Ratio 
        //r6    Price/EPS Estimate Current Year
        //r7    Price/EPS Estimate Next Year
        //s     Symbol
        //s1    Shares Owned
        //s7    Short Ratio
        //t1    Last Trade Time
        //t6    Trade Links
        //t7    Ticker Trend
        //t8	1 yr Target Price
        //v     Volume/td>
        //v1    Holdings Value
        //v7    Holdings Value(Real-time)/td>
        //w	    52-week Range
        //w1    Day’s Value Change
        //w4    Day’s Value Change(Real-time)
        //x     Stock Exchange
        //y     Dividend Yield
        /// </summary>


        public YahooAPIService()
        {
        }


        private string GetWebResponse(string url)
        {
            // Make a WebClient.
            WebClient web_client = new WebClient();

            try { 
                // Get the indicated URL.
                Stream response = web_client.OpenRead(url);

                // Read the result.
                using (StreamReader stream_reader = new StreamReader(response))
                {
                    // Get the results.
                    string result = stream_reader.ReadToEnd();

                    // Close the stream reader and its underlying stream.
                    stream_reader.Close();

                    // Return the result.
                    return result;
                }
            }
            catch (WebException ex)
            {
                throw new NotImplementedException();
            }
            catch (IOException ex)
            {
                throw new NotImplementedException();
            }
            catch (ArgumentException ex)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Lookup ticker and refer to securityDBList for info if needed
        /// </summary>
        /// <param name="tickerToLookUp"></param>
        /// <param name="securityDBList"></param>
        /// <returns></returns>
        public Security GetSingleSecurity(string tickerToLookUp, List<Security> securityDBList)
        {
            
            const string base_url = "http://download.finance.yahoo.com/d/quotes.csv?s=@&f=sl1yj1barvb6a5ncp2";
            Security result;
            Security securityBeingUpdated;
            string securityType = "";

            var url = base_url.Replace("@", tickerToLookUp);
            var response = GetWebResponse(url);
            var yahooResult = new YahooAPIResult(response);
            var isUnknown = IsSecurityUnknown(yahooResult);

            //Ticker is unknown, return blank security.
            if (isUnknown)
                return new Security("", @"N/A", @"Unknown Ticker", 0, 0);

            if (!securityDBList.Any(s => s.Ticker == tickerToLookUp))
            {
                Security newSecurity = CreateNewSecurity(yahooResult);
            }



            securityType = securityDBList.Find(s => s.Ticker == yahooResult.Ticker).SecurityType;

            if (securityType == "Stock")
                securityBeingUpdated = (Stock)securityDBList.Find(s => s.Ticker == yahooResult.Ticker);
            else
                securityBeingUpdated = (MutualFund)securityDBList.Find(s => s.Ticker == yahooResult.Ticker);

            //If securityBeingUpdated is not null then it is known to the DB
            //and its info should be updated.
            if (securityBeingUpdated != null && securityType == "Stock")
            {
                //Fix properties is they came back as NA
                if (yahooResult.PeRatioIsNA) yahooResult.PeRatio = 0;
                if (yahooResult.YieldIsNA) yahooResult.Yield = 0;
                if (yahooResult.LastPriceIsNA) yahooResult.LastPrice = 0;
                yahooResult.MarketCap = yahooResult.MarketCap.Substring(0, yahooResult.MarketCap.Length - 1);

                var stockBeingUpdated = (Stock)securityBeingUpdated;
                stockBeingUpdated.UpdateData(yahooResult);

                return stockBeingUpdated;
            }
            else if (securityBeingUpdated != null && securityType == "Mutual Fund")
            {
                var updatedFund = (MutualFund)securityDBList.Find(s => s.Ticker == yahooResult.Ticker);
                updatedFund.UpdateData(yahooResult);
                return updatedFund;
            }
            else
            {
                throw new NotImplementedException();
            }   
        }

        private Security CreateNewSecurity(YahooAPIResult yahooResult)
        {
            var determinedType = DetermineIfStockOrFund(yahooResult);

            if (determinedType == "Stock")
            {
                yahooResult.MarketCap = yahooResult.MarketCap.Substring(0, yahooResult.MarketCap.Length - 1);
                var newStock = new Stock(yahooResult);
                return newStock;
            }
            else if (determinedType == "Mutual Fund")
            {
                var newFund = new MutualFund(yahooResult);
                return newFund;
            }
            else if (determinedType == "Unknown")
                return new Security("", "N/A", "Unknown Ticker", 0, 0.00);
            else
                return new Security("", "N/A", "Unknown Ticker", 0, 0.00);
        }

        /// <summary>
        /// This is called when the user specifically selects that they want to
        /// buy a stock or mutual fund through the order entry screen
        /// </summary>
        /// <param name="tickerToLookUp"></param>
        /// <param name="securityDBList"></param>
        /// <param name="securityType"></param>
        /// <returns></returns>
        public Security GetSingleSecurity(string tickerToLookUp, List<Security> securityDBList, Security securityType)
        {
            const string base_url = "http://download.finance.yahoo.com/d/quotes.csv?s=@&f=sl1yj1barvb6a5ncp2";            

            var url = base_url.Replace("@", tickerToLookUp);
            var response = GetWebResponse(url);
            var yahooResult = new YahooAPIResult(response);
            
            //If the security is undefined, blank the UI properties and return
            //An unknown stock
            var securityIsUnknown = IsSecurityUnknown(yahooResult);
            if (securityIsUnknown)
            {
                yahooResult.PeRatio = 0;
                yahooResult.Yield = 0;
                yahooResult.Description = "Unknown Ticker";
                yahooResult.LastPrice = 0;
                yahooResult.MarketCap = "0";

                var result = new Stock(yahooResult);
                return result;
            }

            //If ticker is unknown to security DB list, create and return a new one
            if (!securityDBList.Any(s => s.Ticker == tickerToLookUp))
            {
                Security newSecurity = CreateNewSecurity(yahooResult);
                return newSecurity;
            }

            var securityBeingUpdated = securityDBList.Find(s => s.Ticker == yahooResult.Ticker);

            //If securityBeingUpdated is not null then it is known to the DB
            //and its info should be updated.
            if (securityBeingUpdated != null && securityType is Stock)
            {
                //Fix properties is they came back as NA
                if (yahooResult.PeRatioIsNA) yahooResult.PeRatio = 0;
                if (yahooResult.YieldIsNA) yahooResult.Yield = 0;
                if (yahooResult.LastPriceIsNA) yahooResult.LastPrice = 0;
                yahooResult.MarketCap = yahooResult.MarketCap.Substring(0, yahooResult.MarketCap.Length - 1);

                var stockBeingUpdated = (Stock)securityBeingUpdated;
                stockBeingUpdated.UpdateData(yahooResult);

                return stockBeingUpdated;
            }
            else if (securityBeingUpdated != null && securityType is MutualFund)
            {
                var mutualFund = (MutualFund)securityDBList.Find(s => s.Ticker == yahooResult.Ticker);
                mutualFund.UpdateData(yahooResult);
                return mutualFund;
            }
            else 
            {
                    throw new NotImplementedException();
            }
        }


        /// <summary>
        /// Called when updating portfolio prices or checking limits
        /// </summary>
        /// <param name="securities"></param>
        /// <returns></returns>
        public void GetUpdatedPricing(List<Security> securities)
        {
            const string baseUrl = "http://download.finance.yahoo.com/d/quotes.csv?s=@&f=sl1yj1barvb6a5ncp2";

            // Build the URL.
            string tickerString = "";
            foreach (var s in securities)
            {
                tickerString += s.Ticker + "+";
            }

            if (tickerString != "")
            {
                //Remove the trailing plus sign. Faster than comparing
                //for the last item in the foreach loop above and not
                //adding the + at the end
                tickerString = tickerString.Substring(0, tickerString.Length - 1);

                //Prepend the base URL.
                tickerString = baseUrl.Replace("@", tickerString); //Add my tickers to the middle of the url

                //Get the response.
                try
                {
                    //Get the web response and clean it up
                    string response = GetWebResponse(tickerString);
                    string result = Regex.Replace(response, "\\r\\n", "\r\n");

                    //Create an array of the results
                    var yahooResults = CreateYahooAPIResultList(result);


                    //logic for looping through results
                    foreach (var yahooResult in yahooResults)
                    {
                        var securityType = securities.Find(s => s.Ticker == yahooResult.Ticker).SecurityType;
                        if (IsSecurityUnknown(yahooResult))
                        {
                            continue; //do not update security
                        }

                        if (securityType == "Stock")
                        {
                            var stock = (Stock)securities.Find(s => s.Ticker == yahooResult.Ticker);
                            stock.UpdateData(yahooResult);
                            //securitiesToReturn.Add(new Stock(yahooResult));
                        }
                        else if (securityType == "Mutual Fund")
                        {
                            var mutualFund = (MutualFund)securities.Find(s => s.Ticker == yahooResult.Ticker);
                            mutualFund.UpdateData(yahooResult);

                            //string assetClass = mutualFund.AssetClass;
                            //string category = mutualFund.Category;
                            //string subcategory = mutualFund.Subcategory;
                            //securitiesToReturn.Add(new MutualFund(yahooResult, assetClass, category, subcategory));
                        }
                        else
                        {
                            //Unknown security type
                            throw new NotImplementedException();
                        }
                    }
                }

                catch (Exception ex) //Error in parsing Yahoo API results.
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Returns a list of securities based on List of tickers parameter,
        /// but mutual funds will not have asset class, category, or subcategory 
        /// defined. Called by startup methods in PortfolioManagementService
        /// </summary>
        /// <param name="tickers"></param>
        /// <returns></returns>
        public List<Security> GetMultipleSecurities(List<string> tickers)
        {
            const string baseUrl = "http://download.finance.yahoo.com/d/quotes.csv?s=@&f=sl1yj1barvb6a5ncp2";
            var securitiesToReturn = new List<Security>(); //Instantiate the list to return

            // Build the URL.
            string tickerString = "";
            foreach (var ticker in tickers)
            {
                tickerString += ticker + "+";
            }

            if (tickerString != "")
            {
                //Remove the trailing plus sign. Faster than comparing
                //for the last item in the foreach loop above and not
                //adding the + at the end
                tickerString = tickerString.Substring(0, tickerString.Length - 1);

                //Prepend the base URL.
                //s (symbol) n (name) l1 (last price) y (yield) j1 (market cap) 
                //b (bid) a (ask) r (peRatio) a5 (ask size) b6 (bid size)
                tickerString = baseUrl.Replace("@", tickerString); //Add my tickers to the middle of the url

                //Get the response.
                try
                {

                    //Get the web response and clean it up
                    string response = GetWebResponse(tickerString);
                    string result = Regex.Replace(response, "\\r\\n", "\r\n");

                    //Create an array of the results
                    var yahooResults = CreateYahooAPIResultList(result);


                    //logic for looping through results
                    foreach (var yahooResult in yahooResults)
                    {
                        if (IsSecurityUnknown(yahooResult))
                            continue; //do not add stock


                        var securityType = DetermineIfStockOrFund(yahooResult);
                        if (securityType == "Stock")
                        {
                            securitiesToReturn.Add(new Stock(yahooResult));
                        }
                        else if (securityType == "Mutual Fund")
                        {
                            securitiesToReturn.Add(new MutualFund(yahooResult));
                        }
                        else
                        {
                            //Unknown security type & determination error
                            throw new NotImplementedException();
                        }
                    }
                    return securitiesToReturn;
                }
                catch (ArgumentNullException ex) //Error in parsing Yahoo API results.
                {
                    Console.WriteLine(ex.Message);
                    return securitiesToReturn; //probably null
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                    return securitiesToReturn;
                }
            }
            return securitiesToReturn; //probably null
        }    

        private List<YahooAPIResult> CreateYahooAPIResultList(string result)
        {
            var yahooResultList = new List<YahooAPIResult>();

            string fixedResponse = Regex.Replace(result, @"\r\n?|\n", string.Empty);
            string[] lines = result.Split(
                        new char[] { '\r', '\n' },
                        StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var yahooResult = new YahooAPIResult(line);

                if (yahooResult.PeRatioIsNA) yahooResult.PeRatio = 0;
                if (yahooResult.YieldIsNA) yahooResult.Yield = 0;
                if (yahooResult.DescriptionIsNA) yahooResult.Description = "Unknown Ticker";
                if (yahooResult.LastPriceIsNA) yahooResult.LastPrice = 0;
                yahooResult.MarketCap = yahooResult.MarketCap.Substring(0, yahooResult.MarketCap.Length - 1); //removed the amount suffix, e.g. "B"

                yahooResultList.Add(yahooResult);
            }

            return yahooResultList;
        }
       
        private string DetermineIfStockOrFund(YahooAPIResult yahooResult)
        {
            if (yahooResult.MarketCapIsNA && yahooResult.PeRatioIsNA && yahooResult.Ticker.Length == 5)
                return "Mutual Fund";
            else
                return "Stock";
        }

        /// <summary>
        /// Determine based on N/A results if a yahoo result is not known
        /// Bid/Ask and BidSize/AskSize can be N/A after market hours
        /// so they are not used in this determination.
        /// </summary>
        /// <param name="yahooResult"></param>
        /// <returns></returns>
        private bool IsSecurityUnknown(YahooAPIResult yahooResult)
        {
            if (yahooResult.Description == @"N/A")
                return true;

            return false;
        }

        public void Dispose()
        {
        }
    }
}
