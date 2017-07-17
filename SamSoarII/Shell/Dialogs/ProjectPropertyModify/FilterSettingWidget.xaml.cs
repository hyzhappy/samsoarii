using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SamSoarII.Core.Models;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// FilterSettingWidget.xaml 的交互逻辑
    /// </summary>
    public partial class FilterSettingWidget : UserControl, IDisposable
    {
        public FilterSettingWidget(FilterParams _core)
        {
            InitializeComponent();
            core = _core;
            DataContext = core;
        }

        public void Dispose()
        {
            core = null;
            DataContext = null;
        }

        #region Number

        private FilterParams core;
        public FilterParams Core { get { return this.core; } }

        #endregion
    }
}
