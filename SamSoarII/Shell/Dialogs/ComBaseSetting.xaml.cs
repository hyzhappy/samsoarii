using SamSoarII.Core.Communication;
using SamSoarII.Core.Helpers;
using SamSoarII.Core.Models;
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
    /// ComBaseSetting.xaml 的交互逻辑
    /// </summary>
    public partial class ComBaseSetting : UserControl, IDisposable
    {
        public event RoutedEventHandler SettingButtonClick = delegate { };
        public event RoutedEventHandler ModifyButtonClick = delegate { };
        private CommunicationSettingDialogMode mode;
        public CommunicationSettingDialogMode Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
                if(value == CommunicationSettingDialogMode.UPLOAD)
                {
                    GD_Modify.Visibility = Visibility.Collapsed;
                    DataGroupBox.Header = Properties.Resources.Communication_Upload_Data;
                }
            }
        }
        public ComBaseSetting()
        {
            InitializeComponent();
            SettingButton.Click += SettingButton_Click;
        }

        public void Dispose()
        {
            core = null;
        }

        private CommunicationParams core;
        public CommunicationParams Core
        {
            get
            {
                return this.core;
            }
            set
            {
                this.core = value;
                DataContext = value;
                if (Mode == CommunicationSettingDialogMode.DOWNLOAD)
                {
                    CB_Program.IsChecked = DownloadHelper.IsDownloadProgram;
                    CB_Comment.IsChecked = DownloadHelper.IsDownloadComment;
                    CB_Initialize.IsChecked = DownloadHelper.IsDownloadInitialize;
                    CB_Setting.IsChecked = DownloadHelper.IsDownloadSetting;
                }
                else
                {
                    CB_Program.IsChecked = UploadHelper.IsUploadProgram;
                    CB_Comment.IsChecked = UploadHelper.IsUploadComment;
                    CB_Initialize.IsChecked = UploadHelper.IsUploadInitialize;
                    CB_Setting.IsChecked = UploadHelper.IsUploadSetting;
                }
                UpdateDownloadOption();
            }
        }
        private void UpdateDownloadOption()
        {
            CB_Program.IsEnabled = !(CB_Comment.IsChecked == true || CB_Initialize.IsChecked == true);
            if (CB_Comment.IsChecked == true || CB_Initialize.IsChecked == true)
                CB_Program.IsChecked = true;
            if (Mode == CommunicationSettingDialogMode.UPLOAD)
            {
                UploadHelper.UploadOption = 0;
                if (CB_Program.IsChecked == true)
                    UploadHelper.UploadOption &= CommunicationDataDefine.OPTION_PROGRAM;
                if (CB_Comment.IsChecked == true)
                    UploadHelper.UploadOption &= CommunicationDataDefine.OPTION_COMMENT;
                if (CB_Setting.IsChecked == true)
                    UploadHelper.UploadOption &= CommunicationDataDefine.OPTION_SETTING;
                if (CB_Initialize.IsChecked == true)
                    UploadHelper.UploadOption &= CommunicationDataDefine.OPTION_INITIALIZE;
            }
            else
            {
                DownloadHelper.DownloadOption = 0;
                if (CB_Program.IsChecked == true)
                    DownloadHelper.DownloadOption &= CommunicationDataDefine.OPTION_PROGRAM;
                if (CB_Comment.IsChecked == true)
                    DownloadHelper.DownloadOption &= CommunicationDataDefine.OPTION_COMMENT;
                if (CB_Setting.IsChecked == true)
                    DownloadHelper.DownloadOption &= CommunicationDataDefine.OPTION_SETTING;
                if (CB_Initialize.IsChecked == true)
                    DownloadHelper.DownloadOption &= CommunicationDataDefine.OPTION_INITIALIZE;
            }
        }

        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            SettingButtonClick.Invoke(sender, new RoutedEventArgs());
        }
        private void BT_Modify_Click(object sender, RoutedEventArgs e)
        {
            ModifyButtonClick(this, new RoutedEventArgs());
        }
        private void OnChecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
            {
                if (sender == checkbox)
                    SettingButton.IsEnabled = false;
                else
                    UpdateDownloadOption();
            }
            else
            {
                ParamSettingGroupBox.Visibility = Visibility.Visible;
            }
        }
        private void OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
            {
                if (sender == checkbox)
                    SettingButton.IsEnabled = true;
                else
                    UpdateDownloadOption();
            }
            else
            {
                ParamSettingGroupBox.Visibility = Visibility.Hidden;
            }
        }
    }
}
