using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamSoarII.UserInterface
{
    /// <summary>
    /// BaseSetting.xaml 的交互逻辑
    /// </summary>
    public partial class BaseSetting : UserControl
    {
        public event RoutedEventHandler SettingButtonClick;
        public BaseSetting()
        {
            InitializeComponent();
            SettingButton.Click += SettingButton_Click;
        }
        private void SettingButton_Click(object sender, RoutedEventArgs e)
        {
            if (SettingButtonClick != null)
            {
                SettingButtonClick.Invoke(sender ,new RoutedEventArgs());
            }
        }
        private void OnChecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
            {
                SettingButton.IsEnabled = false;
            }
            else
            {
                ParamsSettingStackPanel.Visibility = Visibility.Visible;
            }
        }
        private void OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox)
            {
                SettingButton.IsEnabled = true;
            }
            else
            {
                ParamsSettingStackPanel.Visibility = Visibility.Hidden;
            }
        }
    }
}
