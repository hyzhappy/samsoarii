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
    /// LanaEnsureDialog.xaml 的交互逻辑
    /// </summary>
    public partial class LanaEnsureDialog : Window, IDisposable
    {
        public event RoutedEventHandler EnsureButtonClick;
        public LanaEnsureDialog()
        {
            InitializeComponent();
            KeyDown += LanaEnsureDialog_KeyDown;
            EnsureButton.Click += EnsureButton_Click;
            CancelButton.Click += CancelButton_Click;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void EnsureButton_Click(object sender, RoutedEventArgs e)
        {
            EnsureButtonClick?.Invoke(this, new RoutedEventArgs());
        }

        private void LanaEnsureDialog_KeyDown(object sender, KeyEventArgs e)
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
            this.Close();
        }
    }
}
