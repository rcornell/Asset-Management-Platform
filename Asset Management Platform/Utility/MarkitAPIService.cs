using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Asset_Management_Platform.SecurityClasses;
using Newtonsoft.Json;

namespace Asset_Management_Platform.Utility
{
    //Not currently implemented
    public class MarkitAPIService : IDisposable
    {
        const string baseUri = @"http://dev.markitondemand.com/MODApis/Api/v2/Quote/json?symbol=";

        private MarkitJsonResult GetWebResponse(string ticker)
        {
            var requestUri = baseUri + ticker;
            MarkitJsonResult markitAPIResult = new MarkitJsonResult();
            try
            {
                using (var webClient = new WebClient())
                {
                    var jsonResult = webClient.DownloadString(requestUri);
                    markitAPIResult = JsonConvert.DeserializeObject<MarkitJsonResult>(jsonResult);
                }  
            }
            catch (WebException ex)
            {
                throw new NotImplementedException();
            }
            catch (ArgumentNullException ex)
            {
                throw new NotImplementedException();
            }
            return markitAPIResult;
        }


        private string DetermineSecurityType(MarkitJsonResult markitJsonResult)
        {
            if (markitJsonResult.Description != null)
                return "Stock";
            else
                return "Mutual Fund";
        }

        private Security CreateSecurity(MarkitJsonResult markitJsonResult)
        {
            var newSec = new Security();

            if(markitJsonResult.Description != null)
            {
                newSec = new Stock(markitJsonResult);
            }

            return newSec;
        }

        public Security GetSingleSecurity(string tickerToLookUp, List<Security> securityList)
        {
            var securityToReturn = new Security();

            return securityToReturn;
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
            var securityToReturn = new Security();

            return securityToReturn;
        }

        public List<Security> GetMultipleSecurities(List<Security> securities)
        {
            var listToReturn = new List<Security>();

            return listToReturn;
        }

        /// <summary>
        /// Returns a list of securities based on List of tickers parameter,
        /// but mutual funds will not have asset class, category, or subcategory 
        /// defined.
        /// </summary>
        /// <param name="tickers"></param>
        /// <returns></returns>
        public List<Security> GetMultipleSecurities(List<string> tickers)
        {
            var listToReturn = new List<Security>();

            return listToReturn;
        }

        /// <summary>
        /// Determine based on N/A results if a yahoo result is not known
        /// Bid/Ask and BidSize/AskSize can be N/A after market hours
        /// so they are not used in this determination.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool IsSecurityUnknown(MarkitJsonResult result)
        {
            if (result.Description == null)
                return true;

            return false;
        }

        public void Dispose()
        {
        }

    }
}
