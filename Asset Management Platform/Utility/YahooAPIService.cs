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

        const string _baseUrl = "http://download.finance.yahoo.com/d/quotes.csv?s=@&f=sl1yj1barvb6a5ncp2";

        public YahooAPIService()
        {

        }

        /// <summary>
        /// Lookup ticker and refer to securityDBList for info if needed
        /// </summary>
        /// <param name="tickerToLookUp"></param>
        /// <param name="securityDBList"></param>
        /// <returns></returns>
        public Security GetSingleSecurity(string tickerToLookUp, List<Security> securityDBList)
        {
            Security securityBeingUpdated;

            var yahooRequestUrl = _baseUrl.Replace("@", tickerToLookUp);
            var response = GetWebResponse(yahooRequestUrl);
            var yahooResult = new YahooAPIResult(response);

            //Ticker is unknown, return blank security.
            if (IsSecurityUnknown(yahooResult))
                return new Security("", @"N/A", @"Unknown Ticker", 0, 0);

            //If security database doesn't know the ticker, add it, then return.
            if (!securityDBList.Any(s => s.Ticker == tickerToLookUp))
            {
                Security newSecurity = CreateNewSecurity(yahooResult);
                securityDBList.Add(newSecurity);
                return newSecurity;
            }

            //Find the security, then update.
            var resultSecurity = securityDBList.Find(s => s.Ticker == yahooResult.Ticker);

            if (resultSecurity is Stock)
                securityBeingUpdated = (Stock)securityDBList.Find(s => s.Ticker == yahooResult.Ticker);
            else
                securityBeingUpdated = (MutualFund)securityDBList.Find(s => s.Ticker == yahooResult.Ticker);

            //If securityBeingUpdated is not null then it is known to the DB
            //and its info should be updated.
            if (securityBeingUpdated != null && resultSecurity is Stock)
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
            else if (securityBeingUpdated != null && resultSecurity is MutualFund)
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
                //Remove the trailing plus sign.
                tickerString = tickerString.Substring(0, tickerString.Length - 1);

                //Prepend the base URL.
                tickerString = baseUrl.Replace("@", tickerString); //Add my tickers to the middle of the url

                try
                {

                    //Get the web response and clean it up
                    string response = GetWebResponse(tickerString);
                    string result = Regex.Replace(response, "\\r\\n", "\r\n");

                    //Create a List<T> of the results
                    var yahooResults = CreateYahooAPIResultList(result);

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

        /// <summary>
        /// Called when updating portfolio prices or checking limits
        /// </summary>
        /// <param name="securities"></param>
        /// <returns></returns>
        public void GetUpdatedPricing(List<Security> securities)
        {
            // Build the URL.
            string tickerString = "";
            foreach (var s in securities)
            {
                tickerString += s.Ticker + "+";
            }

            if (tickerString != "")
            {
                //Remove the trailing plus sign.
                tickerString = tickerString.Substring(0, tickerString.Length - 1);

                //Prepend the base URL.
                tickerString = _baseUrl.Replace("@", tickerString); //Add my tickers to the middle of the url

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
                        }
                        else if (securityType == "Mutual Fund")
                        {
                            var mutualFund = (MutualFund)securities.Find(s => s.Ticker == yahooResult.Ticker);
                            mutualFund.UpdateData(yahooResult);
                        }
                        else
                        {
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

        private string GetWebResponse(string url)
        {
            var web_client = new WebClient();

            try
            {
                Stream response = web_client.OpenRead(url);

                using (StreamReader stream_reader = new StreamReader(response))
                {
                    string result = stream_reader.ReadToEnd();

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
            else
                return new Security("", "N/A", "Unknown Ticker", 0, 0.00);
        }

        private List<YahooAPIResult> CreateYahooAPIResultList(string webResult)
        {
            var yahooResultList = new List<YahooAPIResult>();

            string fixedResponse = Regex.Replace(webResult, @"\r\n?|\n", string.Empty);
            string[] lines = webResult.Split(
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
