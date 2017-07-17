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
        bool IsFloat { get; set; }
        LayoutFloatingWindow FloatWindow { get; set; }
        LayoutFloatingWindowControl FloatControl { get; set; }
    }
}
