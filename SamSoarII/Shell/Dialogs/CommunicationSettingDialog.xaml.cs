using SamSoarII.Core.Models;
using SamSoarII.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using SamSoarII.Core.Communication;

namespace SamSoarII.Shell.Dialogs
{
    public enum CommunicationSettingDialogMode
    {
        SETTING, TEST, MONITOR, DOWNLOAD
    }
    /// <summary>
    /// CommunicationSettingDialog_.xaml 的交互逻辑
    /// </summary>
    public partial class CommunicationSettingDialog : Window, ISaveDialog, IDisposable
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
            baseSetting.Core = communicationParams;
            //baseSetting.radiobutton.IsChecked = communicationParams.IsComLinked;
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
            Ensure = null;
            CommunicationTest = null;
            if (IsEnabled) Close();
        }
        public ComBaseSetting GetBaseSetting()
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
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            Save();
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
            int dopt = 0;
            if (baseSetting.CB_Program.IsChecked.Value)
                dopt |= CommunicationDataDefine.OPTION_PROGRAM;
            if (baseSetting.CB_Element.IsChecked.Value)
                dopt |= CommunicationDataDefine.OPTION_ELEMENT;
            if (baseSetting.CB_Initialize.IsChecked.Value)
                dopt |= CommunicationDataDefine.OPTION_INITIALIZE;
            if (baseSetting.CB_Setting.IsChecked.Value)
                dopt |= CommunicationDataDefine.OPTION_SETTING;
            communicationParams.IsComLinked = (bool)baseSetting.radiobutton.IsChecked;
            communicationParams.IsAutoCheck = (bool)baseSetting.checkbox.IsChecked;
            communicationParams.DownloadOption = dopt;
        }
    }
}
