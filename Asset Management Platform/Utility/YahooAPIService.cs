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
        //p6    Price/Book                              l1yj1bara5b6
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

        private List<Security> _securitiesWithMarketData;

        public YahooAPIService()
        {
        }


        private string GetWebResponse(string url)
        {
            // Make a WebClient.
            WebClient web_client = new WebClient();

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


        public Security GetSingleSecurity(string tickerToLookUp, List<Security> securityList)
        {
            
            const string base_url = "http://download.finance.yahoo.com/d/quotes.csv?s=@&f=sl1yj1barvb6a5n";
            Security result;
            Security securityBeingUpdated;
            string securityType = "";

            var url = base_url.Replace("@", tickerToLookUp);
            var response = GetWebResponse(url);
            var yahooResult = CreateYahooAPIResult(response);

            


            if (securityList.Any(s => s.Ticker == yahooResult.Ticker)) {
                securityBeingUpdated = securityList.Find(s => s.Ticker == yahooResult.Ticker);
                securityType = securityBeingUpdated.SecurityType;

                if (securityType == "Stock")
                {
                    var stockIsUnknown = IsStockUnknown(yahooResult);

                    if (stockIsUnknown)
                    {
                        if (yahooResult.PeRatioIsNA) yahooResult.PeRatio = 0;
                        if (yahooResult.YieldIsNA) yahooResult.Yield = 0;
                        if (yahooResult.DescriptionIsNA) yahooResult.Description = "Unknown Ticker";
                        if (yahooResult.LastPriceIsNA) yahooResult.LastPrice = 0;
                        
                        yahooResult.MarketCap = yahooResult.MarketCap.Substring(0, yahooResult.MarketCap.Length - 1); //removed the amount suffix, e.g. "B"

                        result = new Stock(yahooResult);
                        return result;
                    }
                    else //Can you get here?
                    {
                        if (yahooResult.PeRatioIsNA) yahooResult.PeRatio = 0;
                        if (yahooResult.YieldIsNA) yahooResult.Yield = 0;
                        if (yahooResult.DescriptionIsNA) yahooResult.Description = "Unknown Ticker";
                        if (yahooResult.LastPriceIsNA) yahooResult.LastPrice = 0;

                        yahooResult.MarketCap = yahooResult.MarketCap.Substring(0, yahooResult.MarketCap.Length - 1);

                        var stockBeingUpdated = (Stock)securityBeingUpdated;
                        stockBeingUpdated.LastPrice = yahooResult.LastPrice;
                        stockBeingUpdated.PeRatio = yahooResult.PeRatio;
                        stockBeingUpdated.Yield = yahooResult.Yield;
                        stockBeingUpdated.Volume = yahooResult.Volume;
                        stockBeingUpdated.Bid = yahooResult.Bid;
                        stockBeingUpdated.Ask = yahooResult.Ask;
                        stockBeingUpdated.AskSize = yahooResult.AskSize;
                        stockBeingUpdated.BidSize = yahooResult.BidSize;
                        stockBeingUpdated.MarketCap = double.Parse(yahooResult.MarketCap);
                        stockBeingUpdated.Description = yahooResult.Description;
                        stockBeingUpdated.Ticker = yahooResult.Ticker; //Match?

                        return stockBeingUpdated;
                    }
                }
                else if (securityType == "Mutual Fund")
                {
                    var mutualFund = (MutualFund)securityList.Find(s => s.Ticker == yahooResult.Ticker);

                    mutualFund.LastPrice = yahooResult.LastPrice;
                    mutualFund.Description = yahooResult.Description;
                    mutualFund.Yield = yahooResult.Yield;
                    return mutualFund;
                }
                else
                {
                    //Known unknown security?
                    throw new NotImplementedException();
                }
            }
            else //Security is unknown, figure out what it is.
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
        }

        /// <summary>
        /// This is called when the user specifically selects that they want to
        /// buy a stock or mutual fund through the order entry screen
        /// </summary>
        /// <param name="tickerToLookUp"></param>
        /// <param name="securityList"></param>
        /// <param name="securityType"></param>
        /// <returns></returns>
        public Security GetSingleSecurity(string tickerToLookUp, List<Security> securityList, Security securityType)
        {

            const string base_url = "http://download.finance.yahoo.com/d/quotes.csv?s=@&f=sl1yj1barvb6a5n";
            Security result;
            Security securityBeingUpdated;

            var url = base_url.Replace("@", tickerToLookUp);
            var response = GetWebResponse(url);
            var yahooResult = CreateYahooAPIResult(response);




            if (securityList.Any(s => s.Ticker == yahooResult.Ticker))
            {
                securityBeingUpdated = securityList.Find(s => s.Ticker == yahooResult.Ticker);

                if (securityType is Stock)
                {
                    var stockIsUnknown = IsStockUnknown(yahooResult);

                    if (stockIsUnknown)
                    {
                        if (yahooResult.PeRatioIsNA) yahooResult.PeRatio = 0;
                        if (yahooResult.YieldIsNA) yahooResult.Yield = 0;
                        if (yahooResult.DescriptionIsNA) yahooResult.Description = "Unknown Ticker";
                        if (yahooResult.LastPriceIsNA) yahooResult.LastPrice = 0;

                        yahooResult.MarketCap = yahooResult.MarketCap.Substring(0, yahooResult.MarketCap.Length - 1); //removed the amount suffix, e.g. "B"

                        result = new Stock(yahooResult);
                        return result;
                    }
                    else //Can you get here?
                    {
                        if (yahooResult.PeRatioIsNA) yahooResult.PeRatio = 0;
                        if (yahooResult.YieldIsNA) yahooResult.Yield = 0;
                        if (yahooResult.DescriptionIsNA) yahooResult.Description = "Unknown Ticker";
                        if (yahooResult.LastPriceIsNA) yahooResult.LastPrice = 0;

                        yahooResult.MarketCap = yahooResult.MarketCap.Substring(0, yahooResult.MarketCap.Length - 1);

                        var stockBeingUpdated = (Stock)securityBeingUpdated;
                        stockBeingUpdated.LastPrice = yahooResult.LastPrice;
                        stockBeingUpdated.PeRatio = yahooResult.PeRatio;
                        stockBeingUpdated.Yield = yahooResult.Yield;
                        stockBeingUpdated.Volume = yahooResult.Volume;
                        stockBeingUpdated.Bid = yahooResult.Bid;
                        stockBeingUpdated.Ask = yahooResult.Ask;
                        stockBeingUpdated.AskSize = yahooResult.AskSize;
                        stockBeingUpdated.BidSize = yahooResult.BidSize;
                        stockBeingUpdated.MarketCap = double.Parse(yahooResult.MarketCap);
                        stockBeingUpdated.Description = yahooResult.Description;
                        stockBeingUpdated.Ticker = yahooResult.Ticker; //Match?

                        return stockBeingUpdated;
                    }
                }
                else if (securityType is MutualFund)
                {
                    var mutualFund = (MutualFund)securityList.Find(s => s.Ticker == yahooResult.Ticker);

                    mutualFund.LastPrice = yahooResult.LastPrice;
                    mutualFund.Description = yahooResult.Description;
                    mutualFund.Yield = yahooResult.Yield;
                    return mutualFund;
                }
                else
                {
                    //Known unknown security?
                    throw new NotImplementedException();
                }
            }
            else //Security is unknown, figure out what it is.
            {
                //See if this code works properly
                if (securityType is Stock)
                {
                    yahooResult.MarketCap = yahooResult.MarketCap.Substring(0, yahooResult.MarketCap.Length - 1);
                    var newStock = new Stock(yahooResult);
                    return newStock;
                }
                else if (securityType is MutualFund)
                {
                    var newFund = new MutualFund(yahooResult);
                    return newFund;
                }
                else
                    throw new NotImplementedException();
            }
        }

        private YahooAPIResult CreateYahooAPIResult(string result)
        {
            string fixedResponse = Regex.Replace(result, @"\r\n?|\n", string.Empty);

            string ticker = "";
            string description = "";
            decimal lastPrice;
            double yield = 0;
            double bid = 0;
            double ask = 0;
            string marketCap = "0";
            double peRatio = 0;
            int volume = 0;
            int bidSize = 0;
            int askSize = 0;

            bool descriptionIsNA = false;
            bool lastPriceIsNA = false;
            bool yieldIsNA = false;
            bool bidIsNA = false;
            bool askIsNA = false;
            bool marketCapIsNA = false;
            bool peRatioIsNA = false;
            bool volumeIsNA = false;
            bool bidSizeIsNA = false;
            bool askSizeIsNA = false;

            if (string.IsNullOrEmpty(fixedResponse.Split(',')[0]))
                ticker = ""; //Why are you here?
            else
                ticker = fixedResponse.Split(',')[0].Replace("\"", "");

            lastPriceIsNA = !decimal.TryParse(fixedResponse.Split(',')[1], out lastPrice);
            yieldIsNA = !double.TryParse(fixedResponse.Split(',')[2], out yield);
            if (fixedResponse.Split(',')[3] == "N/A")
            {
                marketCapIsNA = true;
                marketCap = "0.0B";
            }
            else
            {
                marketCapIsNA = false;
                marketCap = fixedResponse.Split(',')[3];
            }
            bidIsNA = !double.TryParse(fixedResponse.Split(',')[4], out bid);
            askIsNA = !double.TryParse(fixedResponse.Split(',')[5], out ask);
            peRatioIsNA = !double.TryParse(fixedResponse.Split(',')[6], out peRatio);
            volumeIsNA = !int.TryParse(fixedResponse.Split(',')[7], out volume);
            bidSizeIsNA = !int.TryParse(fixedResponse.Split(',')[8], out bidSize);
            askSizeIsNA = !int.TryParse(fixedResponse.Split(',')[9], out askSize);

            //Index out of bounds?
            //Some Descriptions are split by a comma, e.g. ",Inc."
            //So the method searches for an extra item and appends it
            if (string.IsNullOrEmpty(fixedResponse.Split(',')[10]))
                descriptionIsNA = true;
            else
                description = fixedResponse.Split(',')[10].Replace("\"", "");
            if (fixedResponse.Split(',').Length == 12)
                description += fixedResponse.Split(',')[11].Replace("\"", "");



            return new YahooAPIResult(ticker, description, lastPrice, yield, bid, ask, marketCap,
                                      peRatio, volume, bidSize, askSize, descriptionIsNA, lastPriceIsNA, 
                                      yieldIsNA, bidIsNA, askIsNA,marketCapIsNA, peRatioIsNA, volumeIsNA,
                                      bidSizeIsNA, askSizeIsNA);
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
                var yahooResult = CreateYahooAPIResult(line);

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
            if (yahooResult.Description == @"N/A")
                return "Unknown";

            if (yahooResult.MarketCapIsNA && yahooResult.PeRatioIsNA)
                return "Mutual Fund";
            else
                return "Stock";
        }

        public List<Security> GetMultipleSecurities(List<Security> securities)
        {           
            const string baseUrl = "http://download.finance.yahoo.com/d/quotes.csv?s=@&f=sl1yj1barvb6a5n";

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
                //s (symbol) n (name) l1 (last price) y (yield) j1 (market cap) 
                //b (bid) a (ask) r (peRatio) a5 (ask size) b6 (bid size)
                tickerString = baseUrl.Replace("@", tickerString); //Add my tickers to the middle of the url

                //Get the response.
                try
                {
                    _securitiesWithMarketData = new List<Security>(); //Instantiate the list to return

                    //Get the web response and clean it up
                    string response = GetWebResponse(tickerString);
                    string result = Regex.Replace(response, "\\r\\n", "\r\n");

                    //Create an array of the results
                    var yahooResults = CreateYahooAPIResultList(result);

                    //logic for looping through results
                    foreach (var yahooResult in yahooResults)
                    {
                        var securityType = securities.Find(s => s.Ticker == yahooResult.Ticker).SecurityType;

                        if (securityType == "Stock")
                        {
                            if (IsStockUnknown(yahooResult))
                            {
                                continue; //do not add stock
                            }
                            _securitiesWithMarketData.Add(new Stock(yahooResult));
                        }
                        else if (securityType == "Mutual Fund")
                        {
                            var mutualFund = (MutualFund)securities.Find(s => s.Ticker == yahooResult.Ticker);

                            string assetClass = mutualFund.AssetClass;
                            string category = mutualFund.Category;
                            string subcategory = mutualFund.Subcategory;

                            _securitiesWithMarketData.Add(new MutualFund(yahooResult, assetClass, category, subcategory));

                        }
                        else
                        {
                            //Unknown security type
                            throw new NotImplementedException();
                        }



                    }
                    
                    

                    return _securitiesWithMarketData;
                }

                catch (Exception ex) //Error in parsing Yahoo API results.
                {
                    Console.WriteLine(ex.Message);
                    return _securitiesWithMarketData; //probably null
                }
            }

            return _securitiesWithMarketData; //probably null
        }

        /// <summary>
        /// Determine based on N/A results if a yahoo result is not known
        /// Bid/Ask and BidSize/AskSize can be N/A after market hours
        /// so they are not used in this determination.
        /// </summary>
        /// <param name="yahooResult"></param>
        /// <returns></returns>
        private bool IsStockUnknown(YahooAPIResult yahooResult)
        {
            int numberOfNA = 0;

            if (yahooResult.LastPriceIsNA) numberOfNA++;
            if (yahooResult.YieldIsNA) numberOfNA++;
            if (yahooResult.MarketCapIsNA) numberOfNA++;
            if (yahooResult.PeRatioIsNA) numberOfNA++;
            if (yahooResult.VolumeIsNA) numberOfNA++;

            if (numberOfNA > 2)
                return true;

            return false;
        }

        public void Dispose()
        {
            _securitiesWithMarketData = null;
        }
    }
}
