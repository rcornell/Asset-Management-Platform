using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asset_Management_Platform
{
    public class FixedIncome : Security
    {


        private double _coupon;
        private string _issuer;
        private string _rating;

        public double Coupon
        {
            get
            {
                return _coupon; 
            }
            set
            {
                _coupon = value;
            }            
        }
        public string Issuer
        {
            get
            {
                return _issuer;
            }
            set
            {
                _issuer = value;
            }

        }
        public string Rating
        {
            get
            {
                return _rating;
            }
            set
            {
                _rating = value;
            }

        }

        public string RatingDescription {
            get { return GetRatingDescription(); }
        }

        public FixedIncome(string cusip, string ticker, string description, decimal lastPrice, double yield) 
            : base(cusip, ticker, description, lastPrice, yield)
        {

        }

        public FixedIncome(string cusip, string ticker, string description, decimal lastPrice, double yield, double coupon, string issuer, string rating)
            : base(cusip, ticker, description, lastPrice, yield)
        {
            Coupon = coupon;
            Issuer = issuer;
            Rating = rating;
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
