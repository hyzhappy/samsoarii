using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xceed.Wpf.AvalonDock.Layout;

namespace SamSoarII.Shell.Windows
{
    public enum TabAction { ACTIVE, CLOSE, SELECT, VIEWMODE };

    public interface ITabItem : IFloat, IDisposable, INotifyPropertyChanged
    {
        bool IsTab { get; }
        string TabHeader { get; }
        LayoutDocument TabContainer { get; set; }
        MainTabControl TabControl { get; }
        void Invoke(TabAction action);
    }
}
