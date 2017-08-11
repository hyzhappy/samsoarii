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

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// PasswordDialog.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordDialog : Window
    {
        public string Password = string.Empty;
        public event RoutedEventHandler EnsureButtonClick;
        public PasswordDialog()
        {
            InitializeComponent();
            KeyDown += PasswordDialog_KeyDown;
            EnsureButton.Click += EnsureButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        private void PasswordDialog_KeyDown(object sender, KeyEventArgs e)
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

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void EnsureButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)CB_show.IsChecked) Password = TB_box.Text;
            else Password = PW_box.Password;
            EnsureButtonClick?.Invoke(this, new RoutedEventArgs());
        }

        private void ShowPassword(object sender, RoutedEventArgs e)
        {
            TB_box.Text = PW_box.Password;
            PW_box.Visibility = Visibility.Hidden;
            TB_box.Visibility = Visibility.Visible;
        }

        private void HidePassword(object sender, RoutedEventArgs e)
        {
            PW_box.Password = TB_box.Text;
            TB_box.Visibility = Visibility.Hidden;
            PW_box.Visibility = Visibility.Visible;
        }
    }
}