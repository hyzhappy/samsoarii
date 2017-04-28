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
    /// AddNewRoutineWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AddNewRoutineWindow : Window, IDisposable
    {
        public event RoutedEventHandler EnsureButtonClick;
        public string NameContent
        {
            get
            {
                return NameTextBox.Text;
            }
        }

        public AddNewRoutineWindow()
        {
            InitializeComponent();
            EnsureButton.Click += EnsureButton_Click;
            CancelButton.Click += CancelButton_Click;
            KeyDown += Window_KeyDown;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void EnsureButton_Click(object sender, RoutedEventArgs e)
        {
            if(EnsureButtonClick != null)
            {
                EnsureButtonClick.Invoke(this, new RoutedEventArgs());
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                EnsureButton.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            }
            else
            {
                if(e.Key == Key.Escape)
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
