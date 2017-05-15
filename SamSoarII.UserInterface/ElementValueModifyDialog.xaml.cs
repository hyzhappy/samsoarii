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
    /// ElementValueModifyDialog.xaml 的交互逻辑
    /// </summary>
    public partial class ElementValueModifyDialog : Window, IDisposable
    {
        public event RoutedEventHandler ForceButtonClick;
        public event RoutedEventHandler BitButtonClick;
        public event RoutedEventHandler WordButtonClick;
        public ElementValueModifyDialog()
        {
            InitializeComponent();
            CloseButton.Click += CloseButton_Click;
            KeyDown += ElementValueModifyDialog_KeyDown;
        }

        private void ElementValueModifyDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                CloseButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public void Dispose()
        {
            Close();
        }
        private void OnForceClick(object sender, RoutedEventArgs e)
        {
            if (ForceButtonClick != null)
            {
                ForceButtonClick.Invoke(this,new RoutedEventArgs());
            }
        }
        private void OnBitButtonClick(object sender, RoutedEventArgs e)
        {
            if (BitButtonClick != null)
            {
                BitButtonClick.Invoke(this,new RoutedEventArgs());
            }
        }
        private void OnWordButtonClick(object sender, RoutedEventArgs e)
        {
            if (WordButtonClick != null)
            {
                WordButtonClick.Invoke(this, new RoutedEventArgs());
            }
        }
    }
}
