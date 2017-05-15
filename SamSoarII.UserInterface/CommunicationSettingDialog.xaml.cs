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
    /// <summary>
    /// CommunicationSettingDialog_.xaml 的交互逻辑
    /// </summary>
    public partial class CommunicationSettingDialog : Window,ISaveDialog
    {
        private CommunicationParams communicationParams;
        public CommunicationSettingDialog(CommunicationParams communicationParams)
        {
            InitializeComponent();
            this.communicationParams = communicationParams;
            baseSetting.DataContext = this.communicationParams;
            EnsureButton.Click += EnsureButton_Click;
            CancelButton.Click += CancelButton_Click;
            CommunicationTestButton.Click += CommunicationTestButton_Click;
            KeyDown += CommunicationSettingDialog_KeyDown;
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
        private void CommunicationTestButton_Click(object sender, RoutedEventArgs e)
        {
            //wait to check
            if ((bool)baseSetting.radiobutton.IsChecked)
            {
                
            }
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void EnsureButton_Click(object sender, RoutedEventArgs e)
        {
            //wait to check
            Save();
            Close();
        }
        public void Save()
        {
            communicationParams.IsCOMLinked = (bool)baseSetting.radiobutton.IsChecked;
            communicationParams.IsAutoCheck = (bool)baseSetting.checkbox.IsChecked;
        }
    }
}
