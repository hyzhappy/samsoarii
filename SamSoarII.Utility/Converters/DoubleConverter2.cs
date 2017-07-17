using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SamSoarII.Utility
{
    [ValueConversion(typeof(double), typeof(double))]
    public class DoubleConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double newvalue = (double)value - 30;
            return newvalue;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double newvalue = (double)value + 30;
            return newvalue;
        }
    }
}
