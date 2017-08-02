using SamSoarII.Core.Models;
using SamSoarII.Utility;
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

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// AnalogQuantitySettingWidget.xaml 的交互逻辑
    /// </summary>
    public partial class AnalogQuantitySettingWidget : UserControl, IDisposable
    {
        public AnalogQuantitySettingWidget(AnalogQuantityParams _core)
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

        private AnalogQuantityParams core;
        public AnalogQuantityParams Core { get { return this.core; } }

        #endregion

        private void IP_Channel_Changed(object sender, SelectionChangedEventArgs e)
        {
            switch (IP_Channel.SelectedIndex)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    IP_SP.Visibility = Visibility.Visible;
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                    IP_SP.Visibility = Visibility.Hidden;
                    break;
            }
        }
    }
}
