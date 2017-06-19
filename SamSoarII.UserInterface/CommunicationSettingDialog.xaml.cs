using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SamSoarII.UserInterface
{
    public enum CommunicationSettingDialogMode
    {
        SETTING, TEST, MONITOR, DOWNLOAD 
    }
    /// <summary>
    /// CommunicationSettingDialog_.xaml 的交互逻辑
    /// </summary>
    public partial class CommunicationSettingDialog : Window,ISaveDialog, IDisposable
    {
        public CommunicationSettingDialogMode Mode { get; private set; }
        private CommunicationParams communicationParams;

        public CommunicationSettingDialog(
            CommunicationParams communicationParams,
            CommunicationSettingDialogMode mode = CommunicationSettingDialogMode.SETTING)
        {
            InitializeComponent();
            this.communicationParams = communicationParams;
            Mode = mode;
            baseSetting.DataContext = this.communicationParams;
            EnsureButton.Click += EnsureButton_Click;
            CancelButton.Click += CancelButton_Click;
            CommunicationTestButton.Click += CommunicationTestButton_Click;
            KeyDown += CommunicationSettingDialog_KeyDown;
            if (Mode == CommunicationSettingDialogMode.DOWNLOAD)
            {
                baseSetting.DownloadDataGroupBox.Visibility = Visibility.Visible;
            }
        }
        public void Dispose()
        {
            if (IsEnabled) Close();
        }
        public BaseSetting GetBaseSetting()
        {
            return baseSetting;
        }
        private void CommunicationSettingDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EnsureButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            else
            {
                if (e.Key == Key.Escape)
                {
                    CancelButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                }
            }
        }
        public event RoutedEventHandler CommunicationTest = delegate { };
        private void CommunicationTestButton_Click(object sender, RoutedEventArgs e)
        {
            CommunicationTest?.Invoke(this, e);
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public event RoutedEventHandler Ensure = delegate { };
        private void EnsureButton_Click(object sender, RoutedEventArgs e)
        {
            //wait to check
            Save();
            Ensure(this, e);
        }
        public void Save()
        {
            communicationParams.IsCOMLinked = (bool)baseSetting.radiobutton.IsChecked;
            communicationParams.IsAutoCheck = (bool)baseSetting.checkbox.IsChecked;
            int dopt = 0;
            if (baseSetting.CB_Program.IsChecked.Value)
                dopt |= CommunicationParams.OPTION_PROGRAM;
            if (baseSetting.CB_Comment.IsChecked.Value)
                dopt |= CommunicationParams.OPTION_COMMENT;
            if (baseSetting.CB_Initialize.IsChecked.Value)
                dopt |= CommunicationParams.OPTION_INITIALIZE;
            if (baseSetting.CB_Monitor.IsChecked.Value)
                dopt |= CommunicationParams.OPTION_MONITOR;
            if (baseSetting.CB_Setting.IsChecked.Value)
                dopt |= CommunicationParams.OPTION_SETTING;
            communicationParams.DownloadOption = dopt;
        }
    }
}
