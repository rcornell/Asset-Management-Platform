using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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

        private List<Security> _securities;

        private List<string> _tickers;
        //public List<string> Tickers
        //{
        //    get { return _tickers; }
        //    set { _tickers = value; }
        //}

        public YahooAPIService(List<string> tickers)
        {
            _tickers = tickers;
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
        private void GetData()
        {

            // Build the URL.
            string url = "";
            int i = 0;

            foreach (string s in _tickers)
            {
                url += s[i] + "+";
                i++;
            }

            if (url != "")
            {
                // Remove the trailing plus sign.
                url = url.Substring(0, url.Length - 1);

                // Prepend the base URL.
                const string base_url =
                    "http://download.finance.yahoo.com/d/quotes.csv?s=@&f=sl1d1t1c1";
                url = base_url.Replace("@", url); //Add my tickers to the middle of the url

                // Get the response.
                try
                {
                    _securities = new List<Security>(); //Instantiate the list to return

                    // Get the web response.
                    string result = GetWebResponse(url);
                    Console.WriteLine(result.Replace("\\r\\n", "\r\n"));

                    // Pull out the current prices.
                    string[] lines = result.Split(
                        new char[] { '\r', '\n' },
                        StringSplitOptions.RemoveEmptyEntries);

                    //
                    //Should find a way to determine at runtime whether each item is a stock, mutual fund, or ETF.
                    //Compare tickers to list of tickers that determine which ticker is which type of security

                    foreach (string line in lines)
                    {



                        string cusip = "";
                        string ticker = "";
                        string description = "";
                        float lastPrice = 0;
                        double yield = 0;

                        cusip  = decimal.Parse(lines[0].Split(',')[1]).ToString("C3"); //these were for controls, repurposing for strings to pass to lists maybe
                        ticker = decimal.Parse(lines[1].Split(',')[1]).ToString("C3");
                        description  = decimal.Parse(lines[2].Split(',')[1]).ToString("C3");
                        //lastPrice = float.Parse(lines[3].Split(',')[1]).ToString("C3"));
                        //yield = double.Parse(lines[3].Split(',')[1]).ToString("C3");

                        _securities.Add(new Security(cusip, ticker, description, lastPrice, yield));
                    }


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadKey(true);
                }
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
