using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using Xceed.Wpf.AvalonDock.Controls;

namespace Xceed.Wpf.AvalonDock.Converters
{
    public class SelectedItemMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            LayoutDocumentPaneControl paneControl = (LayoutDocumentPaneControl)values[0];
            TabItem item = (TabItem)((ContentPresenter)values[1]).TemplatedParent;
            return item.Header;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
