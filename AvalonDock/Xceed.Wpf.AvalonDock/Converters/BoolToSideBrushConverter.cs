using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Xceed.Wpf.AvalonDock.Converters
{
    [ValueConversion(typeof(bool), typeof(SolidColorBrush))]
    public class BoolToSideBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                bool bvalue = (bool)(value);
                if (bvalue)
                {
                    return Brushes.Blue;
                }
                else
                {
                    return Brushes.Black;
                }
            }
            else
            {
                return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush)
            {
                SolidColorBrush scbrush = (SolidColorBrush)(value);
                if (scbrush.Equals(Brushes.Blue))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
