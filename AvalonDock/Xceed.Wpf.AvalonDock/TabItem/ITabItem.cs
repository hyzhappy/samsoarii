using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace SamSoarII.AppMain.Project
{
    public interface ITabItem
    {
        string TabHeader { get; set; }
        double ActualWidth { get; set; }
        double ActualHeight { get; set; }
        bool IsFloat { get; set; }
        LayoutFloatingWindow FloatWindow { get; set; }
        LayoutFloatingWindowControl FloatControl { get; set; }
        event RoutedEventHandler FloatClosed;
    }
}
