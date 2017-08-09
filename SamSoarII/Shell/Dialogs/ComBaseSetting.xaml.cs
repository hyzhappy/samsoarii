using SamSoarII.Core.Communication;
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
        
        public ComBaseSetting()
        {
            InitializeComponent();
            SettingButton.Click += SettingButton_Click;
        }

        public void Dispose()
        {

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
                if (core != null)
                {
                    CB_Program.IsChecked = core.IsDownloadProgram;
                    CB_Element.IsChecked = core.IsDownloadElement;
                    CB_Initialize.IsChecked = core.IsDownloadInitialize;
                    CB_Setting.IsChecked = core.IsDownloadSetting;
                    UpdateDownloadOption();
                }
            }
        }
        private void UpdateDownloadOption()
        {
            int downloadoption = 0;
            if (core.IsDownloadProgram)
                downloadoption |= CommunicationDataDefine.OPTION_PROGRAM;
            if (core.IsDownloadElement)
                downloadoption |= CommunicationDataDefine.OPTION_ELEMENT;
            if (core.IsDownloadInitialize)
                downloadoption |= CommunicationDataDefine.OPTION_INITIALIZE;
            if (core.IsDownloadSetting)
                downloadoption |= CommunicationDataDefine.OPTION_SETTING;
            core.DownloadOption = downloadoption;
            CB_Program.IsEnabled = !(CB_Element.IsChecked == true);
            if (CB_Element.IsChecked == true || CB_Setting.IsChecked == true)
                CB_Program.IsChecked = true;
            core.DownloadOption = 0;
            if (CB_Program.IsChecked == true)
                core.DownloadOption &= CommunicationDataDefine.OPTION_PROGRAM;
            if (CB_Element.IsChecked == true)
                core.DownloadOption &= CommunicationDataDefine.OPTION_ELEMENT;
            if (CB_Setting.IsChecked == true)
                core.DownloadOption &= CommunicationDataDefine.OPTION_SETTING;
            if (CB_Initialize.IsChecked == true)
                core.DownloadOption &= CommunicationDataDefine.OPTION_INITIALIZE;
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
