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
