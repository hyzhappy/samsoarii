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
    public partial class ComBaseSetting : UserControl
    {
        public event RoutedEventHandler SettingButtonClick = delegate { };
        public event RoutedEventHandler ModifyButtonClick = delegate { };

        private int datalen;
        public int DataLen
        {
            get
            {
                return this.datalen;
            }
            set
            {
                this.datalen = value;
                TB_Memory.Text = String.Format("{0}{1:N2} KB",
                    Properties.Resources.Memory_Used,
                    ((double)datalen) / 1024);
            }
        }

        public ComBaseSetting()
        {
            InitializeComponent();
            SettingButton.Click += SettingButton_Click;
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
            }
            else
            {
                ParamSettingGroupBox.Visibility = Visibility.Hidden;
            }
        }
    }
}
