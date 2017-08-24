using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xceed.Wpf.AvalonDock.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace SamSoarII.Shell.Windows
{
    public interface IFloat
    {
        bool IsFloat { get; }
        LayoutFloatingWindowControl FloatControl { get; set; }
        event RoutedEventHandler FloatOpened;
        event RoutedEventHandler FloatClosed;
        bool IsViewThreadActive { get; }
        void ViewThreadStart();
        void ViewThreadPause();
        event RoutedEventHandler ViewThreadPaused; 
    }
}
