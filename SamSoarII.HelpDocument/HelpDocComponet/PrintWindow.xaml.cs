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

namespace SamSoarII.HelpDocument.HelpDocComponet
{
    /// <summary>
    /// PrintWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PrintWindow : Window,IDisposable
    {
        public event RoutedEventHandler EnsureButtonClick;
        public PrintWindow()
        {
            InitializeComponent();
            CancelButton.Click += CancelButton_Click;
            EnsureButton.Click += EnsureButton_Click;
            KeyDown += PrintWindow_KeyDown;
        }
        public bool IsRecursion
        {
            get
            {
                return (bool)PrintRecursion.IsChecked;
            }
        }
        private void PrintWindow_KeyDown(object sender, KeyEventArgs e)
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

        private void EnsureButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnsureButtonClick != null)
            {
                EnsureButtonClick.Invoke(this, new RoutedEventArgs());
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
