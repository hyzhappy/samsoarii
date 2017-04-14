using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Controls;

namespace BetterTab.Converters
{
    public class WrapVisualConverter : IValueConverter
    {
        #region Methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var fe = value as FrameworkElement;
            if (fe != null)
            {
                return new Rectangle() { Width = fe.ActualWidth, Height = fe.ActualHeight, Fill = new VisualBrush(fe) { Stretch = Stretch.None }, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left };
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}
