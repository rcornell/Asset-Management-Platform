using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    class FixedIncome : Security
    {
        private double _coupon;
        public double Coupon { get; set; }

        private string _issuer;
        public string Issuer { get; set; }

        private string _rating;
        public string Rating { get; set; }

        public string RatingDescription {
            get { return GetRatingDescription(); }
        }


        public FixedIncome(string cusip, string ticker, string description, float lastPrice, double yield) 
            : base(cusip, ticker, description, lastPrice, yield)
        {

        }

        public FixedIncome(string cusip, string ticker, string description, float lastPrice, double yield, double coupon, string issuer, string rating)
            : base(cusip, ticker, description, lastPrice, yield)
        {
            _coupon = coupon;
            _issuer = issuer;
            _rating = rating;
        }

        public string GetRatingDescription()
        {
            switch (_rating) {
                case "D":
                    return "In Default";
                case "C":
                case "CC":
                case "CCC-":
                    return "Default Imminent";
                case "CCC":
                    return "Extremely Speculative";
                case "CCC+":
                    return "Substantial Risks";
                case "B-":
                case "B":
                case "B+":
                    return "Highly Speculative";
                case "BB-":
                case "BB":
                case "BB+":
                    return "Non-Investment Grade Speculative";
                case "BBB-":
                case "BBB":
                case "BBB+":
                    return "Lower Medium Grade";
                case "A-":
                case "A":
                case "A+":
                    return "Upper Medium Grade";
                case "AA-":
                case "AA":
                case "AA+":
                    return "High Grade";
                case "AAA":
                    return "Prime";
                default:
                    return "Unknown";
            }
        }
    }
}
