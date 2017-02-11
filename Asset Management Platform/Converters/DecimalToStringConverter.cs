using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Asset_Management_Platform.Converters
{
    public class DecimalToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null) { 
                var dec = (decimal) value;
                var formattedDecimal = dec.ToString("#,##0");
                return formattedDecimal;
            }
            return "0"; //Will only hit this if the passed in value aram is null
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //Won't need this.
            throw new NotImplementedException();
        }
    }
}
