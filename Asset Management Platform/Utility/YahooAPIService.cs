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

            var url = base_url.Replace("@", tickerToLookUp);

            var response = GetWebResponse(url);
            
            string fixedResponse = Regex.Replace(response, @"\r\n?|\n", string.Empty);

            string securityType = "";
            //cusip = "" no CUSIP ability in the Yahoo API. Load via my own database?
            string ticker = "";
            string description = "";
            string cusip = " ";
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

            //Some stock names contain a comma, which is the character that 
            //we are using to split up the results. The below code accounts for that possibility
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
            if (string.IsNullOrEmpty(fixedResponse.Split(',')[10]))
                descriptionIsNA = true;
            else
                description = fixedResponse.Split(',')[10].Replace("\"", "");
            if (fixedResponse.Split(',').Length == 12)
                description += fixedResponse.Split(',')[11].Replace("\"", "");

            if (securityList.Any(s => s.Ticker == ticker))
                securityType = securityList.Find(s => s.Ticker == ticker).SecurityType;


            Security result;
            if (securityType == "Stock") {
                if (IsStockUnknown(lastPriceIsNA, yieldIsNA, marketCapIsNA, bidIsNA, askIsNA, peRatioIsNA, volumeIsNA, bidSizeIsNA, askSizeIsNA, descriptionIsNA))
                {
                    var desc = !descriptionIsNA ? description : "Unknown Ticker";
                    var price = !lastPriceIsNA ? lastPrice : 0;
                    var yld = !yieldIsNA ? yield : 0.00;

                    if (peRatioIsNA) peRatio = 0;
                    if (yieldIsNA) yield = 0;

                    marketCap = marketCap.Substring(0, marketCap.Length - 1); //removed the amount suffix, e.g. "B"
                    var floatMarketCap = float.Parse(marketCap); //you should fix this

                    result = new Stock("", ticker, desc, price, yld);
                    return result;
                }
            }
            else if (securityType == "Mutual Fund")
            {
                var mutualFund = (MutualFund)securityList.Find(s => s.Ticker == ticker);

                string assetClass = mutualFund.AssetClass;
                string category = mutualFund.Category;
                string subcategory = mutualFund.Subcategory;

                //Don't return a new MutualFund. Modify what the _securitiesWithMarketData list contains.
                //result = new MutualFund(cusip, ticker, description, lastPrice, yield, assetClass, category, subcategory));
                mutualFund.LastPrice = lastPrice;
                mutualFund.Description = description;
                mutualFund.Yield = yield;
                return mutualFund;
            }
            else { 
                //Unknown mutual fund?
                throw new NotImplementedException();
            }

            return new Stock("", "XXX", "Unknown Security", 0, 0.00);
             //new Stock(cusip, ticker, description, lastPrice, yield, bid, ask, floatMarketCap, peRatio, volume, bidSize, askSize);
        }

        public List<Security> GetMultipleSecurities(List<Security> securities)
        {           
            const string baseUrl = "http://download.finance.yahoo.com/d/quotes.csv?s=@&f=sl1yj1barvb6a5n";

            // Build the URL.
            string tickerString = "";
            int j = 0; //index for looping through array of results from yahooAPI
            string[] lines;
            
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

                    //Get the web response.
                    string response = GetWebResponse(tickerString);

                    string result = Regex.Replace(response, "\\r\\n", "\r\n");
                    

                    //Create an array of the results
                    lines = result.Split(
                        new char[] { '\r', '\n' },
                        StringSplitOptions.RemoveEmptyEntries);

                    //Should find a way to determine at runtime whether each item is a stock, mutual fund, or ETF.
                    //Compare tickers to list of tickers that determine which ticker is which type of security
                    //Use LINQ?

                    foreach (string line in lines)
                    {
                        string ticker = "";
                        string description = "";
                        string cusip = "";
                        string securityType = "";
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
                        
                        
                        //cusip = "" no CUSIP ability in this API. Load via my own database
                        //Some stock names contain a comma, which is the character that 
                        //we are using to split up the results. The below code accounts for that possibility
                        System.Diagnostics.Debug.Write(string.Format("Creating Security #{0}", j));


                        if (string.IsNullOrEmpty(lines[j].Split(',')[0]))
                            ticker = "";
                        else
                            ticker = lines[j].Split(',')[0].Replace("\"", "");

                        lastPriceIsNA = !decimal.TryParse(lines[j].Split(',')[1], out lastPrice);
                        if (j == 496)
                        {
                            System.Diagnostics.Debug.Write("Waiting");
                        }
                        yieldIsNA = !double.TryParse(lines[j].Split(',')[2], out yield);
                        if (lines[j].Split(',')[3] == "N/A") {
                            marketCapIsNA = true;
                            marketCap = "0.0B";
                        }
                        else
                        {
                            marketCapIsNA = false;
                            marketCap = lines[j].Split(',')[3];
                        }

                        
                        bidIsNA = !double.TryParse(lines[j].Split(',')[4], out bid);
                        askIsNA = !double.TryParse(lines[j].Split(',')[5], out ask);                           
                        peRatioIsNA = !double.TryParse(lines[j].Split(',')[6], out peRatio);
                        volumeIsNA = !int.TryParse(lines[j].Split(',')[7], out volume);
                        bidSizeIsNA = !int.TryParse(lines[j].Split(',')[8], out bidSize);
                        askSizeIsNA = !int.TryParse(lines[j].Split(',')[9], out askSize);
                        if (string.IsNullOrEmpty(lines[j].Split(',')[10]))
                            descriptionIsNA = true;
                        else
                            description = lines[j].Split(',')[10].Replace("\"", "" );
                        if(lines[j].Split(',').Length == 12)
                            description += lines[j].Split(',')[11].Replace("\"", "");

                            
                        if (peRatioIsNA) peRatio = 0;
                        if (yieldIsNA) yield = 0;

                        marketCap = marketCap.Substring(0, marketCap.Length - 1); //removed the amount suffix, e.g. "B"
                        var floatMarketCap = float.Parse(marketCap); //you should fix this

                        securityType = securities.Find(s => s.Ticker == ticker).SecurityType;

                        if (securityType == "Stock")
                        {
                            if (IsStockUnknown(lastPriceIsNA, yieldIsNA, marketCapIsNA, bidIsNA, askIsNA, peRatioIsNA, volumeIsNA, bidSizeIsNA, askSizeIsNA, descriptionIsNA))
                            {
                                j++;
                                continue; //do not add stock
                            }
                            _securitiesWithMarketData.Add(new Stock(cusip, securities[j].Ticker, description, lastPrice, yield, bid, ask, floatMarketCap, peRatio, volume, bidSize, askSize));
                        }
                        else if (securityType == "Mutual Fund")
                        {
                            var mutualFund = (MutualFund)securities.Find(s => s.Ticker == ticker);

                            string assetClass = mutualFund.AssetClass;
                            string category = mutualFund.Category;
                            string subcategory = mutualFund.Subcategory;

                            _securitiesWithMarketData.Add(new MutualFund(cusip, ticker, description, lastPrice, yield, assetClass, category, subcategory));

                        }
                        else
                            //Unknown security type
                            throw new NotImplementedException();
                            
                        j++; //advance to next security
                        
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

        private bool IsStockUnknown(bool lastPriceIsNA, bool yieldIsNA, bool marketCapIsNA, bool bidIsNA, bool askIsNA, bool peRatioIsNA, bool volumeIsNA, bool bidSizeIsNA, bool askSizeIsNA, bool descriptionIsNA)
        {
            int numberOfNA = 0;

            if (lastPriceIsNA) numberOfNA++;
            if (yieldIsNA) numberOfNA++;
            if (marketCapIsNA) numberOfNA++;
            //if (bidIsNA) numberOfNA++; these can be N/A after market hours
            //if (askIsNA) numberOfNA++;
            if (peRatioIsNA) numberOfNA++;
            if (volumeIsNA) numberOfNA++;
            //if (bidSizeIsNA) numberOfNA++; these can be N/A after market hours
            //if (askSizeIsNA) numberOfNA++;

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
