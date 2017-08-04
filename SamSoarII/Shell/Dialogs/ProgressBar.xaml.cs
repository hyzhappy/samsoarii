using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SamSoarII.Shell.Dialogs
{
    /// <summary>
    /// ProgressBar.xaml 的交互逻辑
    /// </summary>
    public partial class ProgressBar : Window,INotifyPropertyChanged
    {
        public static readonly DependencyProperty MessageProperty;
        static ProgressBar()
        {
            MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(LoadingWindow));
        }
        public string Message
        {
            get
            {
                return (string)GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }
        private double _percentage;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public double Percentage
        {
            get
            {
                return _percentage;
            }
            set
            {
                _percentage = value;
                PropertyChanged(this,new PropertyChangedEventArgs("Percentage"));
            }
        }
        public ProgressBar()
        {
            InitializeComponent();
            DataContext = this;
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }

        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    }
}
