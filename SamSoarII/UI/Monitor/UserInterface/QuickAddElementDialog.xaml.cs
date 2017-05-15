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

namespace SamSoarII.AppMain.UI.Monitor
{
    /// <summary>
    /// QuickAddElementDialog.xaml 的交互逻辑
    /// </summary>
    public partial class QuickAddElementDialog : Window, IDisposable
    {
        public event RoutedEventHandler EnsureButtonClick;
        public QuickAddElementDialog()
        {
            InitializeComponent();
            KeyDown += QuickAddElementDialog_KeyDown;
            EnsureButton.Click += EnsureButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void EnsureButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnsureButtonClick != null)
            {
                EnsureButtonClick.Invoke(this, new RoutedEventArgs());
            }
        }

        private void QuickAddElementDialog_KeyDown(object sender, KeyEventArgs e)
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
        public void Dispose()
        {
            Close();
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            combox.IsEnabled = true;
        }

        private void OnUnchecked(object sender, RoutedEventArgs e)
        {
            combox.IsEnabled = false;
            combox.SelectedIndex = 0;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (textbox1 != null && textbox2 != null)
            {
                if (combox.SelectedIndex == 0)
                {
                    textbox1.IsEnabled = false;
                    textbox2.IsEnabled = false;
                }
                else
                {
                    textbox1.IsEnabled = true;
                    textbox2.IsEnabled = true;
                }
            }
        }
    }
}
