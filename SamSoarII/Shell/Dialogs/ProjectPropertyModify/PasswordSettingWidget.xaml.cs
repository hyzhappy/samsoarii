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
    /// PasswordSettingWidget.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordSettingWidget : UserControl, IDisposable
    {
        public PasswordSettingWidget(PasswordParams _core)
        {
            InitializeComponent();
            core = _core;
            DataContext = core;
            Initialized += PasswordSettingWidget_Initialized;
        }

        public void Dispose()
        {
            core = null;
            DataContext = null;
        }

        private PasswordParams core;
        public PasswordParams Core { get { return this.core; } }

        private void PasswordSettingWidget_Initialized(object sender, EventArgs e)
        {
            UP_Box.Password = core.PWUpload;
            DP_Box.Password = core.PWDownload;
            MP_Box.Password = core.PWMonitor;
        }

        public void Save()
        {
            core.PWUpload = UP_Box.Password;
            core.PWDownload = DP_Box.Password;
            core.PWMonitor = MP_Box.Password;
        }
    }
}
